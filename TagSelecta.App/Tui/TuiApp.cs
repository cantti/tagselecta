using System.Threading.Channels;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Rendering;
using TagSelecta.App.IO;
using TagSelecta.App.Tui.TuiCommands;
using TagSelecta.App.Tui.Widgets;
using TagSelecta.Tagging;

namespace TagSelecta.App.Tui;

public class TuiApp(
    IAnsiConsole console,
    IAudioFileScanner audioFileScanner,
    HotkeyMap hotkeys,
    IUserActionReader userActionReader,
    ITuiCommandFactory commandFactory
) : AsyncCommand<TuiSettings>, ITuiCommandContext
{
    private const string HeaderLayoutKey = "navigation";
    private const string FilesLayoutKey = "files";
    private const string TagDataLayoutKey = "body";
    private const string CommandLayoutKey = "command";
    private const string StatusLayoutKey = "status";

    private string _statusMessage = "";

    public IEnumerable<TagDataOperation> Operations { get; private set; } = [];

    public IEnumerable<TagDataOperation> VisibleOperations =>
        Operations.Where(x => !FilterEnabled || x.HasChanges);

    public IEnumerable<TagDataOperation> SelectedOperations =>
        VisibleOperations.Any(x => x.IsSelected) ? VisibleOperations.Where(x => x.IsSelected)
        : FocusedOperation is not null ? new[] { FocusedOperation }
        : Enumerable.Empty<TagDataOperation>();

    public int FocusedOperationIndex { get; set; }

    public TagDataOperation? FocusedOperation =>
        VisibleOperations.ElementAtOrDefault(FocusedOperationIndex);

    private CancellationTokenSource _cts = new();

    private CancellationTokenSource? _currentCommandCts;
    private Task _currentCommandTask = Task.CompletedTask;

    public bool TreeEnabled { get; set; }
    public bool FilterEnabled { get; set; }
    public bool HelpEnabled { get; set; }

    protected override async Task<int> ExecuteAsync(
        CommandContext context,
        TuiSettings settings,
        CancellationToken ct
    )
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);

        BindHotkeys();

        if (!ValidateOptions(context))
        {
            return 1;
        }

        AltScreen.Enter();

        Operations = audioFileScanner
            .ScanAndRead(settings.Path)
            .Select(x => new TagDataOperation(x.Path, x.TagData))
            .ToList();

        var channel = Channel.CreateUnbounded<ConsoleKeyInfo>();

        _ = StartInputLoop(channel);
        try
        {
            await StartUiLoop(channel);
        }
        catch (OperationCanceledException) { }

        AltScreen.Exit();

        return 0;
    }

    private Task StartUiLoop(Channel<ConsoleKeyInfo> channel)
    {
        return console
            .Live(new Panel("Starting..."))
            .AutoClear(true)
            .StartAsync(async ctx =>
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    ctx.UpdateTarget(DrawLayout());
                    while (channel.Reader.TryRead(out var key))
                    {
                        if (userActionReader.TryRead(key, out var request))
                        {
                            await DispatchCommand(request);
                        }
                        ctx.UpdateTarget(DrawLayout());
                    }
                    await Task.Delay(33, _cts.Token);
                }
            });
    }

    private Task StartInputLoop(Channel<ConsoleKeyInfo> channel)
    {
        return Task.Run(() =>
        {
            try
            {
                while (true)
                {
                    var key = Console.ReadKey(intercept: true);
                    if (_currentCommandTask.IsCompleted || key.Key == ConsoleKey.Escape)
                    {
                        channel.Writer.TryWrite(key);
                    }
                }
            }
            catch
            {
                // ignored
            }
        });
    }

    private IRenderable DrawLayout()
    {
        var filesContentSize = Math.Min(
            (Console.WindowHeight - 3 - 1 - 1 - 1) / 2,
            VisibleOperations.Count() + 2
        );

        FocusedOperationIndex = Math.Clamp(
            FocusedOperationIndex,
            0,
            Math.Max(0, VisibleOperations.Count() - 1)
        );

        var layout = new Layout("root").SplitRows(
            new Layout(HeaderLayoutKey).Size(3).Update(RenderHeader()),
            new Layout(FilesLayoutKey)
                .Size(filesContentSize)
                .Update(
                    HelpEnabled ? new HelpWidget()
                    : TreeEnabled
                        ? new TreeListWidget(
                            VisibleOperations,
                            FocusedOperation,
                            filesContentSize - 2
                        )
                    : new FileListWidget(
                        VisibleOperations,
                        FocusedOperationIndex,
                        filesContentSize - 2
                    )
                ),
            new Layout(TagDataLayoutKey).Ratio(1).Update(Text.Empty),
            new Layout(StatusLayoutKey).Size(1).Update(new StatusWidget(_statusMessage)),
            new Layout(CommandLayoutKey)
                .Size(1)
                .Update(
                    userActionReader.Mode == InputMode.Command
                        ? new CommandPromptWidget(userActionReader.Text, userActionReader.CursorPos)
                        : Text.Empty
                )
        );

        if (FocusedOperation is not null)
        {
            var tagDataRenderable = FocusedOperation.HasChanges
                ? TagDataPrinter.PrintComparison(FocusedOperation)
                : TagDataPrinter.PrintTagData(FocusedOperation);

            if (FocusedOperation.Exception is null)
            {
                layout[TagDataLayoutKey].Update(tagDataRenderable);
            }
            else
            {
                layout[TagDataLayoutKey]
                    .Update(
                        new Rows(
                            tagDataRenderable,
                            Text.NewLine,
                            new Text(
                                $"Error: {FocusedOperation.Exception.Message}",
                                new Style(Color.Red)
                            )
                        )
                    );
            }
        }
        return layout;
    }

    private bool ValidateOptions(CommandContext context)
    {
        var unknownOptions = context
            .Remaining.Parsed.Select(kvp => kvp.Key)
            .Concat(context.Remaining.Raw)
            .ToList();
        if (unknownOptions.Count != 0)
        {
            console.MarkupLine($"[red]Unknown option(s) provided:[/]");
            foreach (var option in unknownOptions)
            {
                console.WriteLine($"  {option}", new Style(Color.Yellow));
            }
            return false;
        }
        return true;
    }

    private IRenderable RenderHeader()
    {
        var keys = new List<(string Key, string Action)> { ("q", "Quit"), ("h", "Help") };
        var cols1 = keys.Select(x => new Markup($"[bold blue]{x.Key}[/] ➔ {x.Action}")).ToList();
        return new Rows(
            new Text("Tagselecta:", new Style(Color.Yellow)),
            new Columns(cols1) { Padding = new Padding(2, 0, 2, 0), Expand = false }
        );
    }

    public void Quit()
    {
        _cts.Cancel();
    }

    private readonly Lock _printLock = new();

    public void Print(string markupMessage)
    {
        lock (_printLock)
        {
            _statusMessage = markupMessage;
        }
    }

    private void BindHotkeys()
    {
        hotkeys.Bind(ConsoleKey.Escape, "clearselection");
        hotkeys.Bind(ConsoleKey.J, "movedown");
        hotkeys.Bind(ConsoleKey.DownArrow, "movedown");
        hotkeys.Bind(ConsoleKey.K, "moveup");
        hotkeys.Bind(ConsoleKey.UpArrow, "moveup");
        hotkeys.Bind(ConsoleKey.Q, "quit");
        hotkeys.Bind(ConsoleKey.T, "toggletree");
        hotkeys.Bind(ConsoleKey.F, "togglefilter");
        hotkeys.Bind(ConsoleKey.H, "togglehelp");
        hotkeys.Bind(ConsoleKey.U, "undo");
        hotkeys.Bind(ConsoleKey.Tab, "select");
        hotkeys.Bind(ConsoleKey.Spacebar, "select");
        hotkeys.Bind(ConsoleKey.A, "selectall");
        hotkeys.Bind(ConsoleKey.Multiply, "selectall");
        hotkeys.Bind(ConsoleKey.W, "write");
    }

    private async Task WaitCurrentCommand(bool isCancelRequest)
    {
        if (_currentCommandCts is null)
        {
            return;
        }
        if (isCancelRequest)
        {
            Print("Cancelling...");
            await _currentCommandCts.CancelAsync();
        }
        try
        {
            await _currentCommandTask;
        }
        catch (OperationCanceledException)
        {
            // expected
        }
        if (isCancelRequest)
        {
            Print("Cancelled.");
        }
        _currentCommandCts.Dispose();
    }

    private async Task DispatchCommand(Request request)
    {
        var isCancelRequest = request.Name == "cancel";

        await WaitCurrentCommand(isCancelRequest);

        if (isCancelRequest)
        {
            return;
        }

        _currentCommandCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token);

        var command = commandFactory.Create(request.Name);

        if (command is not null)
        {
            _currentCommandTask = ExecuteCommand(
                command.ExecuteAsync(this, request, _currentCommandCts.Token)
            );
        }
    }

    private async Task ExecuteCommand(Task task)
    {
        hotkeys.Bind(ConsoleKey.Escape, "cancel");
        try
        {
            await task;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            // todo log Exception
        }
        finally
        {
            hotkeys.Bind(ConsoleKey.Escape, "clearselection");
        }
    }
}
