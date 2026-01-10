using System.Threading.Channels;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Rendering;
using TagSelecta.Cli.IO;
using TagSelecta.Cli.Tui.TuiCommands;
using TagSelecta.Cli.Tui.Widgets;
using TagSelecta.Tagging;

namespace TagSelecta.Cli.Tui;

public class TuiApp(
    IAnsiConsole console,
    IAudioFileScanner audioFileScanner,
    ITagger tagger,
    HotkeyMap hotkeys,
    IUserActionReader userActionReader,
    IFileSystem fs,
    ITuiCommandFactory commandFactory
) : AsyncCommand<TuiSettings>, ITuiCommandContext
{
    private const string HeaderLayoutKey = "navigation";
    private const string FilesLayoutKey = "files";
    private const string TagDataLayoutKey = "body";
    private const string CommandLayoutKey = "command";

    private string _statusMessage = "";
    private bool _inputBlocked = false;

    private List<TagDataOperation> _operations = [];

    public IEnumerable<TagDataOperation> VisibleOperations =>
        _operations.Where(x => !FilterEnabled || x.HasChanges);

    public IEnumerable<TagDataOperation> SelectedOperations =>
        VisibleOperations.Count(x => x.IsSelected) > 0 ? VisibleOperations.Where(x => x.IsSelected)
        : FocusedOperation is not null ? new[] { FocusedOperation }
        : Enumerable.Empty<TagDataOperation>();

    public int FocusedOperationIndex { get; set; }

    public TagDataOperation? FocusedOperation =>
        VisibleOperations.ElementAtOrDefault(FocusedOperationIndex);

    private CancellationTokenSource _cts = new();

    private CancellationTokenSource? _currentCommandCts;
    private Task? _currentCommandTask;

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

        _operations = audioFileScanner
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
                    channel.Writer.TryWrite(key);
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
        var navigationSize = 3;

        var filesContentSize = Math.Min(
            (Console.WindowHeight - navigationSize) / 2,
            VisibleOperations.Count() + 2
        );

        var layout = new Layout("root").SplitRows(
            new Layout(HeaderLayoutKey).Size(3).Update(RenderHeader()),
            new Layout(FilesLayoutKey).Size(filesContentSize).Update(Text.Empty),
            new Layout(TagDataLayoutKey).Ratio(1).Update(Text.Empty),
            // todo rewrite
            new Layout("status")
                .Size(1)
                .Update(
                    new Markup($" {_statusMessage}{(_inputBlocked ? ". Press c to cancel." : "")}")
                ),
            new Layout(CommandLayoutKey)
                .Size(1)
                .Update(
                    userActionReader.Mode == InputMode.Command
                        ? new CommandPromptWidget(userActionReader.Text, userActionReader.CursorPos)
                        : Text.Empty
                )
        );

        FocusedOperationIndex = Math.Clamp(
            FocusedOperationIndex,
            0,
            Math.Max(0, VisibleOperations.Count() - 1)
        );

        IRenderable fileListContent =
            HelpEnabled ? new HelpWidget()
            : TreeEnabled
                ? new TreeListWidget(VisibleOperations, FocusedOperation, filesContentSize - 2)
            : new FileListWidget(VisibleOperations, FocusedOperationIndex, filesContentSize - 2);

        layout[FilesLayoutKey].Update(fileListContent);

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

    private void Undo()
    {
        FocusedOperation?.Undo();
    }

    private void Write()
    {
        console.Clear();
        var operationsToWrite = VisibleOperations.Where(x => x.HasChanges).ToList();
        console
            .Progress()
            .Start(ctx =>
            {
                var task = ctx.AddTask("Writing metadata...", maxValue: operationsToWrite.Count);
                for (var i = 0; i < operationsToWrite.Count; i++)
                {
                    var operation = operationsToWrite[i];
                    operation.Write(tagger, fs);
                    task.Description =
                        $"Writing metadata {i + 1} of {operationsToWrite.Count}({operation.CurrentPath.EscapeMarkup()})";
                    task.Increment(1);
                }
            });
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

    private void SelectAll()
    {
        foreach (var operation in VisibleOperations)
        {
            operation.IsSelected = true;
        }
    }

    private void ClearSelection()
    {
        foreach (var operation in _operations)
        {
            operation.IsSelected = false;
        }
    }

    private void BindHotkeys()
    {
        hotkeys.Bind(ConsoleKey.C, "cancel");
        hotkeys.Bind(ConsoleKey.J, "movedown");
        hotkeys.Bind(ConsoleKey.K, "moveup");
        hotkeys.Bind(ConsoleKey.Q, "quit");
        hotkeys.Bind(ConsoleKey.T, "toggletree");
        hotkeys.Bind(ConsoleKey.F, "togglefilter");
        hotkeys.Bind(ConsoleKey.H, "togglehelp");
        hotkeys.Bind(ConsoleKey.U, UiAction.Undo);
        hotkeys.Bind(ConsoleKey.Tab, UiAction.Select);
        hotkeys.Bind(ConsoleKey.V, UiAction.Select);
        hotkeys.Bind(ConsoleKey.Escape, UiAction.ClearSelection);
        hotkeys.Bind(ConsoleKey.A, UiAction.SelectAll);
        hotkeys.Bind(ConsoleKey.W, UiAction.Write);
    }

    private async Task DispatchCommand(Request request)
    {
        if (_inputBlocked && request.Name != "cancel")
        {
            return;
        }

        if (_currentCommandCts != null)
        {
            if (request.Name == "cancel")
            {
                Print("Cancelling...");
                await _currentCommandCts.CancelAsync();
            }
            try
            {
                await _currentCommandTask!;
            }
            catch (OperationCanceledException)
            {
                // expected
            }
            _currentCommandCts.Dispose();
        }

        _currentCommandCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token);

        var command = commandFactory.Create(request.Name);

        if (command is not null)
        {
            if (command.BlockInput)
            {
                _inputBlocked = true;
            }

            _currentCommandTask = SafeExecuteAsync(
                command.ExecuteAsync(this, request, _currentCommandCts.Token)
            );
        }
        else
        {
            if (request.Name == "cancel")
            {
                Print("Canceled.");
            }
            else
            {
                Print("Command not found.");
            }
            _currentCommandTask = Task.CompletedTask;
        }
    }

    private async Task SafeExecuteAsync(Task task)
    {
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
            _inputBlocked = false;
        }
    }
}
