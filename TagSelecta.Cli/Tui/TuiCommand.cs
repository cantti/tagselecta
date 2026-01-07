using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Rendering;
using TagSelecta.Cli.IO;
using TagSelecta.Tagging;

namespace TagSelecta.Cli.Tui;

public class TuiCommand(
    IAnsiConsole console,
    IAudioFileScanner audioFileScanner,
    ITagger tagger,
    HotkeyMap hotkeys,
    IUserActionReader userActionReader,
    ActionDispatcher actionDispatcher,
    IFileSystem fs
) : AsyncCommand<TuiSettings>
{
    private Dictionary<string, Func<ValueTask>> _handlers = [];

    private const string HeaderLayoutKey = "navigation";
    private const string FilesLayoutKey = "files";
    private const string TagDataLayoutKey = "body";

    private bool _running = true;
    private bool _filterEnabled;
    private bool _treeEnabled;
    private bool _helpEnabled;
    private readonly TreeFileListFactory _treeListFactory = new();

    private List<TagDataOperation> _operations = [];
    private List<TagDataOperation> _visibleOperations = [];

    private int _focusedOperationIndex;
    private TagDataOperation? FocusedOperation =>
        _visibleOperations.ElementAtOrDefault(_focusedOperationIndex);

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

        AltScreen.Enter();

        _operations = _visibleOperations = audioFileScanner
            .ScanAndRead(settings.Path)
            .Select(x => new TagDataOperation(x.Path, x.TagData))
            .ToList();

        console.WriteLine(_operations.Count + " files found");

        UpdateTreeView();

        await AnsiConsole
            .Live(new Panel("Starting..."))
            .AutoClear(true)
            .StartAsync(async ctx =>
            {
                while (_running)
                {
                    ctx.UpdateTarget(RenderConsoleLayout());
                    var request = userActionReader.Read();
                    if (_handlers.TryGetValue(request.ActionName, out var uiAction))
                    {
                        await uiAction();
                    }
                    else
                    {
                        await DispatchAction(request);
                    }
                }
            });

        AltScreen.Exit();

        return 0;
    }

    private IRenderable RenderConsoleLayout()
    {
        var navigationSize = 3;

        var filesContentSize = Math.Min(
            (Console.WindowHeight - navigationSize) / 2,
            _visibleOperations.Count + 2
        );

        var layout = new Layout("root").SplitRows(
            new Layout(HeaderLayoutKey).Size(3).Update(RenderHeader()),
            new Layout(FilesLayoutKey).Size(filesContentSize).Update(Text.Empty),
            new Layout(TagDataLayoutKey).Update(Text.Empty)
        );

        _focusedOperationIndex = Math.Clamp(
            _focusedOperationIndex,
            0,
            Math.Max(0, _visibleOperations.Count - 1)
        );

        var fileListContent =
            _helpEnabled ? RenderHelp()
            : _treeEnabled
                ? _treeListFactory.Render(
                    _visibleOperations,
                    _focusedOperationIndex,
                    filesContentSize - 2,
                    _filterEnabled
                )
            : RenderFileList(
                _visibleOperations,
                _focusedOperationIndex,
                filesContentSize - 2,
                _filterEnabled
            );

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

        // var segments = ((IRenderable)layout).Render(
        //     new RenderOptions(
        //         console.Profile.Capabilities,
        //         new Size(console.Profile.Width, console.Profile.Height)
        //     ),
        //     80
        // );
    }

    private void Undo()
    {
        FocusedOperation?.Undo();
        UpdateTreeView();
    }

    private async Task DispatchAction(ActionRequest action)
    {
        await actionDispatcher.Dispatch(action, null, null, DispatchType.BeforeProcess);

        await console
            .Progress()
            .AutoClear(true)
            .StartAsync(async ctx =>
            {
                var task = ctx.AddTask(
                    "Processing metadata...",
                    maxValue: _visibleOperations.Count
                );
                var progressLock = new object();
                await Parallel.ForEachAsync(
                    _visibleOperations.Where(x => x.IsSelected),
                    async (operation, _) =>
                    {
                        try
                        {
                            await actionDispatcher.Dispatch(
                                action,
                                operation,
                                _visibleOperations,
                                DispatchType.Process
                            );
                            operation.CheckForChanges();
                        }
                        catch (Exception ex)
                        {
                            operation.MarkError(ex);
                        }
                        lock (progressLock)
                        {
                            task.Increment(1);
                        }
                    }
                );
            });
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
        UpdateTreeView();
    }

    private IRenderable RenderFileList(
        List<TagDataOperation> operations,
        int selectedIndex,
        int windowSize,
        bool filter
    )
    {
        if (operations.Count == 0)
        {
            return Text.Empty;
        }

        // center around the current index (5 lines above, 4 below), but keep a full window when possible
        var windowStart = selectedIndex - (windowSize / 2);

        // clamp so we dont go before 0 or past the last possible full window start
        var maxStart = Math.Max(0, operations.Count - windowSize);
        windowStart = Math.Clamp(windowStart, 0, maxStart);

        var linesToPrint = Math.Min(windowSize, operations.Count - windowStart);

        var items = new List<IRenderable>();

        for (var i = 0; i < linesToPrint; i++)
        {
            var itemIndex = windowStart + i;
            var path = Path.GetRelativePath(
                Environment.CurrentDirectory,
                operations[itemIndex].OriginalPath
            );
            var selectedMarker = operations[itemIndex].IsSelected ? "[x]" : "[ ]";
            var text = $"{selectedMarker} {path}";
            text = text.Substring(0, Math.Min(text.Length, Console.WindowWidth))
                .PadRight(Console.WindowWidth);
            var style = new Style(
                operations[itemIndex].HasChanges ? Color.Red : Color.Default,
                selectedIndex == itemIndex ? Color.Grey : Color.Default
            );
            items.Add(new Text(text, style));
        }

        return new Rows(
            new Text(
                $"Files ({operations.Count}{(filter ? ", filtered" : "")}):",
                new Style(Color.Yellow)
            ),
            new Rows(items)
        );
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

    private void UpdateTreeView()
    {
        _treeListFactory.BuildTreeLines(_visibleOperations);
    }

    private void ToggleFilter()
    {
        _filterEnabled = !_filterEnabled;
        _visibleOperations = _operations.Where(x => !_filterEnabled || x.HasChanges).ToList();
        UpdateTreeView();
        _focusedOperationIndex = 0;
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

    private static IRenderable RenderHelp()
    {
        var keys = new List<(string Key, string Action)>
        {
            ("t", "Toggle tree"),
            ("f", "Toggle filter"),
            ("j, move down", "Move down"),
            ("k, move up", "Move up"),
            ("u", "Undo. Only if not written!"),
            (":w", "Write"),
            (":wa", "Write all"),
            (":s", "Set tags (artist=Bach title=\"The Goldberg Variations\" all)"),
            (":at", "Auto track number"),
            (":split", "Split artists"),
            (":tc", "Title case conversion"),
            (":tc", "Extract picture"),
            (
                ":discogs",
                "release=https://www.discogs.com/master/163206-King-Tubby-Presents-The-Roots-Of-Dub"
            ),
        };
        var grid = new Grid();
        grid.AddColumn();
        grid.AddColumn();
        foreach (var key in keys)
        {
            grid.AddRow($"[bold blue]{key.Key}[/]", key.Action);
        }
        return new Rows(new Text("Help:", new Style(Color.Yellow)), grid);
    }

    private void SetUiHandlers()
    {
        _handlers = new()
        {
            [UiAction.MoveDown] = () =>
            {
                _focusedOperationIndex++;
                return ValueTask.CompletedTask;
            },
            [UiAction.MoveUp] = () =>
            {
                _focusedOperationIndex--;
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
                _running = false;
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
        foreach (var operation in _operations)
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

    private void Selection()
    {
        _operations[_focusedOperationIndex].IsSelected = !_operations[
            _focusedOperationIndex
        ].IsSelected;
        _focusedOperationIndex++;
    }

    private void BindHotkeys()
    {
        hotkeys.Bind(ConsoleKey.J, UiAction.MoveDown);
        hotkeys.Bind(ConsoleKey.K, UiAction.MoveUp);
        hotkeys.Bind(ConsoleKey.Q, UiAction.Quit);
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
