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
    private const string HeaderLayoutKey = "navigation";
    private const string FilesLayoutKey = "files";
    private const string TagDataLayoutKey = "body";

    private bool _running = true;
    private int _selectedIndex;
    private bool _filterEnabled;
    private bool _treeEnabled;
    private bool _helpEnabled;
    private readonly TreeFileListFactory _treeListFactory = new();
    private List<TagDataOperation> _operations = [];
    private List<TagDataOperation> _shownOperations = [];
    private TagDataOperation? _selectedOperation;
    private Dictionary<string, Func<ValueTask>> _handlers = [];

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

        _operations = _shownOperations = audioFileScanner
            .ScanAndRead(settings.Path)
            .Select(x => new TagDataOperation(x.Path, x.TagData))
            .ToList();

        console.WriteLine(_operations.Count + " files found");

        UpdateTreeView();

        while (_running)
        {
            await RenderConsoleLayout();
        }

        AltScreen.Exit();

        console.MarkupLineInterpolated(
            $"{_operations.Count(x => x.Status == TagDataOperationStatus.Written)}/{_operations.Count} files written"
        );

        var errorsCount = _operations.Count(x => x.Status == TagDataOperationStatus.Failed);

        if (errorsCount > 0)
        {
            console.MarkupLineInterpolated($"{errorsCount} errors");
        }

        return 0;
    }

    private async Task RenderConsoleLayout()
    {
        console.Clear();

        var navigationSize = 3;

        var filesContentSize = Math.Min(
            (Console.WindowHeight - navigationSize) / 2,
            _shownOperations.Count + 2
        );

        var layout = new Layout("root").SplitRows(
            new Layout(HeaderLayoutKey).Size(3).Update(RenderHeader()),
            new Layout(FilesLayoutKey).Size(filesContentSize).Update(Text.Empty),
            new Layout(TagDataLayoutKey).Update(Text.Empty)
        );

        _selectedIndex = Math.Clamp(_selectedIndex, 0, Math.Max(0, _shownOperations.Count - 1));

        _selectedOperation = _shownOperations.Count > 0 ? _shownOperations[_selectedIndex] : null;

        if (_selectedOperation is not null)
        {
            var fileListContent =
                _helpEnabled ? RenderHelp()
                : _treeEnabled
                    ? _treeListFactory.Render(
                        _shownOperations,
                        _selectedIndex,
                        filesContentSize - 2,
                        _filterEnabled
                    )
                : RenderFileList(
                    _shownOperations,
                    _selectedIndex,
                    filesContentSize - 2,
                    _filterEnabled
                );

            layout[FilesLayoutKey].Update(fileListContent);

            var tagDataRenderable = _selectedOperation.HasChanges
                ? TagDataPrinter.PrintComparison(_selectedOperation)
                : TagDataPrinter.PrintTagData(_selectedOperation);

            if (_selectedOperation.Exception is null)
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
                                $"Error: {_selectedOperation.Exception.Message}",
                                new Style(Color.Red)
                            )
                        )
                    );
            }
        }
        else
        {
            layout[TagDataLayoutKey].Update(new Text("No files found"));
        }

        console.Write(layout);

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

    private void Undo()
    {
        _selectedOperation?.Undo();
        UpdateTreeView();
    }

    private async Task DispatchAction(ActionRequest actionRequest)
    {
        if (actionRequest.Args.Any(x => x.Key.StartsWith("arg") && x.Value == "all"))
        {
            await ProcessAll(_shownOperations, actionRequest);
            UpdateTreeView();
        }
        else
        {
            if (_selectedOperation is not null)
            {
                await actionDispatcher.Dispatch(
                    actionRequest,
                    null,
                    null,
                    DispatchType.BeforeProcess
                );

                await actionDispatcher.Dispatch(
                    actionRequest,
                    _selectedOperation,
                    _shownOperations,
                    DispatchType.Process
                );
                _selectedOperation.CheckForChanges();
                UpdateTreeView();
            }
        }
    }

    private async Task ProcessAll(List<TagDataOperation> operations, ActionRequest action)
    {
        await actionDispatcher.Dispatch(action, null, null, DispatchType.BeforeProcess);

        await console
            .Progress()
            .AutoClear(true)
            .StartAsync(async ctx =>
            {
                var task = ctx.AddTask("Processing metadata...", maxValue: operations.Count);
                var progressLock = new object();
                await Parallel.ForEachAsync(
                    operations,
                    async (operation, _) =>
                    {
                        try
                        {
                            await actionDispatcher.Dispatch(
                                action,
                                operation,
                                operations,
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
        if (_selectedOperation is not { Status: TagDataOperationStatus.Pending, HasChanges: true })
        {
            return;
        }
        _selectedOperation.Write(tagger, fs);
        UpdateTreeView();
    }

    private void WriteAll(List<TagDataOperation> operations)
    {
        console.Clear();
        var operationsToWrite = operations
            .Where(x => x is { Status: TagDataOperationStatus.Pending, HasChanges: true })
            .ToList();
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
            var lineNumber = (itemIndex + 1).ToString().PadLeft(4);
            var modifiedMarker = operations[itemIndex].HasChanges ? "*" : " ";
            var text = $"{lineNumber} {modifiedMarker} {path}";
            text = text.Substring(0, Math.Min(text.Length, Console.WindowWidth))
                .PadRight(Console.WindowWidth);
            var style = new Style(background: selectedIndex == itemIndex ? Color.Grey : null);
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
        _treeListFactory.BuildTreeLines(_shownOperations);
    }

    private void ToggleFilter()
    {
        _filterEnabled = !_filterEnabled;
        _shownOperations = _operations.Where(x => !_filterEnabled || x.HasChanges).ToList();
        UpdateTreeView();
        _selectedIndex = 0;
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
                _selectedIndex++;
                return ValueTask.CompletedTask;
            },
            [UiAction.MoveUp] = () =>
            {
                _selectedIndex--;
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
                WriteAll(_shownOperations);
                return ValueTask.CompletedTask;
            },
            [UiAction.WriteAllAlias] = () =>
            {
                WriteAll(_shownOperations);
                return ValueTask.CompletedTask;
            },
        };
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
    }
}
