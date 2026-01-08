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
    ITuiCommandDispatcher commandDispatcher
) : AsyncCommand<TuiSettings>, ITuiCommandContext
{
    private Dictionary<string, Func<ValueTask>> _handlers = [];

    private const string HeaderLayoutKey = "navigation";
    private const string FilesLayoutKey = "files";
    private const string TagDataLayoutKey = "body";

    private readonly CancellationTokenSource _cts = new();

    private bool _filterEnabled;
    private bool _treeEnabled;
    private bool _helpEnabled;

    private List<TagDataOperation> _visibleOperations = [];

    public List<TagDataOperation> Operations { get; private set; } = [];

    public int FocusedOperationIndex { get; set; }

    public TagDataOperation? FocusedOperation =>
        _visibleOperations.ElementAtOrDefault(FocusedOperationIndex);

    protected override async Task<int> ExecuteAsync(
        CommandContext context,
        TuiSettings settings,
        CancellationToken ct
    )
    {
        BindHotkeys();
        SetUiHandlers();

        if (!ValidateOptions(context))
        {
            return 1;
        }

        // AltScreen.Enter();

        Operations = _visibleOperations = audioFileScanner
            .ScanAndRead(settings.Path)
            .Select(x => new TagDataOperation(x.Path, x.TagData))
            .ToList();

        console.WriteLine(Operations.Count + " files found");

        var channel = Channel.CreateUnbounded<ConsoleKeyInfo>();

        _ = Task.Run(() => InputLoop(channel, _cts.Token), _cts.Token);

        await console
            .Live(new Panel("Starting..."))
            .Overflow(VerticalOverflow.Ellipsis)
            .AutoClear(true)
            .StartAsync(async ctx =>
            {
                AnsiConsole.Cursor.Show();
                ctx.UpdateTarget(DrawLayout());

                while (!_cts.Token.IsCancellationRequested)
                {
                    while (channel.Reader.TryRead(out var key))
                    {
                        var request = userActionReader.Read(key);
                        if (request is not null)
                        {
                            await commandDispatcher.DispatchAsync(this, request);
                        }
                        ctx.UpdateTarget(DrawLayout());
                        if (userActionReader.Mode == InputMode.Command)
                        {
                            Console.SetCursorPosition(
                                userActionReader.Cursor + 1,
                                Console.WindowHeight - 1
                            );
                        }
                    }
                    await Task.Delay(33, _cts.Token);
                }
            });

        AltScreen.Exit();

        return 0;
    }

    static async Task InputLoop(Channel<ConsoleKeyInfo> channel, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var key = Console.ReadKey(intercept: true);
            await channel.Writer.WriteAsync(key, token);
        }
    }

    private IRenderable DrawLayout()
    {
        var navigationSize = 3;

        var filesContentSize = Math.Min(
            (Console.WindowHeight - navigationSize) / 2,
            _visibleOperations.Count + 2
        );

        IRenderable statusBar =
            userActionReader.Mode == InputMode.Command
                ? new Columns(new Text(":"), new Text(userActionReader.Buffer.ToString()))
                {
                    Expand = false,
                    Padding = new Padding(0, 0, 0, 0),
                }
                : Text.Empty;

        var layout = new Layout("root").SplitRows(
            new Layout(HeaderLayoutKey).Size(3).Update(RenderHeader()),
            new Layout(FilesLayoutKey).Size(filesContentSize).Update(Text.Empty),
            new Layout(TagDataLayoutKey).Ratio(1).Update(Text.Empty),
            new Layout("footer").Size(1).Update(statusBar)
        );

        FocusedOperationIndex = Math.Clamp(
            FocusedOperationIndex,
            0,
            Math.Max(0, _visibleOperations.Count - 1)
        );

        IRenderable fileListContent =
            _helpEnabled ? new HelpWidget()
            : _treeEnabled
                ? new TreeListWidget(
                    _visibleOperations,
                    FocusedOperationIndex,
                    filesContentSize - 2
                )
            : new FileListWidget(_visibleOperations, FocusedOperationIndex, filesContentSize - 2);

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
        var operationsToWrite = _visibleOperations.Where(x => x.HasChanges).ToList();
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

    private void ToggleFilter()
    {
        _filterEnabled = !_filterEnabled;
        _visibleOperations = Operations.Where(x => !_filterEnabled || x.HasChanges).ToList();
        FocusedOperationIndex = 0;
    }

    private void ToggleTree()
    {
        _treeEnabled = !_treeEnabled;
    }

    private void ToggleHelp()
    {
        _helpEnabled = !_helpEnabled;
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

    private void SetUiHandlers()
    {
        _handlers = new()
        {
            [UiAction.MoveDown] = () =>
            {
                FocusedOperationIndex++;
                return ValueTask.CompletedTask;
            },
            [UiAction.MoveUp] = () =>
            {
                FocusedOperationIndex--;
                return ValueTask.CompletedTask;
            },
            [UiAction.ToggleTree] = () =>
            {
                ToggleTree();
                return ValueTask.CompletedTask;
            },
            [UiAction.ToggleFilter] = () =>
            {
                ToggleFilter();
                return ValueTask.CompletedTask;
            },
            [UiAction.ToggleHelp] = () =>
            {
                ToggleHelp();
                return ValueTask.CompletedTask;
            },
            [UiAction.Undo] = () =>
            {
                Undo();
                return ValueTask.CompletedTask;
            },
            [UiAction.Quit] = () =>
            {
                _cts.Cancel();
                return ValueTask.CompletedTask;
            },
            [UiAction.Write] = () =>
            {
                Write();
                return ValueTask.CompletedTask;
            },
            [UiAction.WriteAlias] = () =>
            {
                Write();
                return ValueTask.CompletedTask;
            },

            [UiAction.WriteAll] = () =>
            {
                Write();
                return ValueTask.CompletedTask;
            },
            [UiAction.WriteAllAlias] = () =>
            {
                Write();
                return ValueTask.CompletedTask;
            },
            [UiAction.Select] = () =>
            {
                Selection();
                return ValueTask.CompletedTask;
            },
            [UiAction.ClearSelection] = () =>
            {
                ClearSelection();
                return ValueTask.CompletedTask;
            },
            [UiAction.SelectAll] = () =>
            {
                SelectAll();
                return ValueTask.CompletedTask;
            },
        };
    }

    private void SelectAll()
    {
        foreach (var operation in Operations)
        {
            operation.IsSelected = true;
        }
    }

    private void ClearSelection()
    {
        foreach (var operation in Operations)
        {
            operation.IsSelected = false;
        }
    }

    private void Selection()
    {
        Operations[FocusedOperationIndex].IsSelected = !Operations[
            FocusedOperationIndex
        ].IsSelected;
        FocusedOperationIndex++;
    }

    private void BindHotkeys()
    {
        hotkeys.Bind(ConsoleKey.J, "movedown");
        hotkeys.Bind(ConsoleKey.K, "moveup");
        hotkeys.Bind(ConsoleKey.Q, "quit");
        hotkeys.Bind(ConsoleKey.T, UiAction.ToggleTree);
        hotkeys.Bind(ConsoleKey.F, UiAction.ToggleFilter);
        hotkeys.Bind(ConsoleKey.H, UiAction.ToggleHelp);
        hotkeys.Bind(ConsoleKey.U, UiAction.Undo);
        hotkeys.Bind(ConsoleKey.Tab, UiAction.Select);
        hotkeys.Bind(ConsoleKey.V, UiAction.Select);
        hotkeys.Bind(ConsoleKey.Escape, UiAction.ClearSelection);
        hotkeys.Bind(ConsoleKey.A, UiAction.SelectAll);
        hotkeys.Bind(ConsoleKey.W, UiAction.Write);
    }
}
