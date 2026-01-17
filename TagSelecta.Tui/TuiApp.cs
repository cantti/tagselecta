using System.Threading.Channels;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Rendering;
using TagSelecta.Shared.IO;
using TagSelecta.Shared.TagDataActions;
using TagSelecta.Tui.TuiCommands;
using TagSelecta.Tui.Widgets;

namespace TagSelecta.Tui;

public class TuiApp(
    IAnsiConsole console,
    IAudioFileScanner audioFileScanner,
    HotkeyMap hotkeys,
    IRequestReader requestReader,
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
        try
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);

            BindHotkeys();

            if (!ValidateOptions(context))
            {
                return 1;
            }

            AltScreen.Enter();

            Operations = audioFileScanner
                .ScanAndRead(settings.Path, ct)
                .Select(x => new TagDataOperation(x.Path, x.TagData))
                .ToList();

            var channel = Channel.CreateUnbounded<ConsoleKeyInfo>();
            _ = StartInputLoop(channel);
            await StartUiLoop(channel);
        }
        catch (OperationCanceledException)
        {
            // expected
        }
        finally
        {
            AltScreen.Exit();
        }

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
                        if (requestReader.TryRead(key, out var request))
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
        var filesContentSize = (Console.WindowHeight - 3 - 1 - 1 - 1) / 2;

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
                    : new FileListWidget(VisibleOperations, FocusedOperation, filesContentSize - 2)
                ),
            new Layout(TagDataLayoutKey).Ratio(1).Update(new TagDataWidget(FocusedOperation)),
            new Layout(StatusLayoutKey).Size(1).Update(new StatusWidget(_statusMessage)),
            new Layout(CommandLayoutKey)
                .Size(1)
                .Update(
                    requestReader.Mode == InputMode.Command
                        ? new CommandPromptWidget(requestReader.Text, requestReader.CursorPos)
                        : Text.Empty
                )
        );
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

    private async Task DispatchCommand(Request request)
    {
        var isCancelRequest = request.Name == "cancel";

        if (isCancelRequest && _currentCommandCts is not null)
        {
            await _currentCommandCts.CancelAsync();
            _currentCommandCts.Dispose();
        }

        await _currentCommandTask;

        if (isCancelRequest)
        {
            return;
        }

        _currentCommandCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token);

        var command = commandFactory.Create(request.Name);

        hotkeys.Bind(ConsoleKey.Escape, "cancel");

        _currentCommandTask = Task.Run(async () =>
        {
            try
            {
                await command.ExecuteAsync(this, request, _currentCommandCts.Token);
            }
            catch (OperationCanceledException)
            {
                Print("Cancelled.");
            }
            catch (Exception ex)
            {
                Print(ex.Message);
            }
            hotkeys.Bind(ConsoleKey.Escape, "clearselection");
        });
    }
}
