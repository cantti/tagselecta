using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Rendering;
using TagSelecta.Cli.IO;
using TagSelecta.Tagging;

namespace TagSelecta.Cli.Commands.Tui;

public class TuiCommand(
    IAnsiConsole console,
    IAudioFileScanner audioFileScanner,
    ITagger tagger,
    HotkeyMap hotkeys,
    IUserActionReader userActionReader,
    ActionDispatcher actionDispatcher
) : AsyncCommand<TuiSettings>
{
    private const string HeaderLayoutKey = "navigation";
    private const string FilesLayoutKey = "files";
    private const string TagDataLayoutKey = "body";

    // ui actions
    private const string ActionMoveDown = "move_down";
    private const string ActionMoveUp = "move_up";
    private const string ActionQuit = "quit";
    private const string ActionToggleTree = "toggle_tree";
    private const string ActionToggleFilter = "toggle_filter";

    private int _selectedIndex;
    private bool _filterEnabled;
    private bool _treeEnabled;
    private readonly TreeFileListFactory _treeListFactory = new();
    private List<TagDataOperation> _operations = [];
    private List<TagDataOperation> _shownOperations = [];
    private TagDataOperation? _selectedOperation;
    private bool _running = true;

    protected override async Task<int> ExecuteAsync(
        CommandContext context,
        TuiSettings settings,
        CancellationToken ct
    )
    {
        BindHotkeys();

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

    private void BindHotkeys()
    {
        hotkeys.Bind(ConsoleKey.J, ActionMoveDown);
        hotkeys.Bind(ConsoleKey.K, ActionMoveUp);
        hotkeys.Bind(ConsoleKey.Q, ActionQuit);
        hotkeys.Bind(ConsoleKey.T, ActionToggleTree);
        hotkeys.Bind(ConsoleKey.F, ActionToggleFilter);
    }

    private async Task Start() { }

    private async Task RenderConsoleLayout()
    {
        console.Clear();

        // var navigation = userActionReader.RenderNavigation();
        var navigationSize = 3;

        var filesContentSize = Math.Min(
            (Console.WindowHeight - 3) / 2,
            // +2 for navigation and add empty row
            _shownOperations.Count + 2
        );

        var layout = new Layout("root").SplitRows(
            new Layout(HeaderLayoutKey).Size(3).Update(Text.Empty),
            new Layout(FilesLayoutKey).Size(filesContentSize).Update(Text.Empty),
            new Layout(TagDataLayoutKey).Update(Text.Empty)
        );

        _selectedIndex = Math.Clamp(_selectedIndex, 0, Math.Max(0, _shownOperations.Count - 1));

        _selectedOperation = _shownOperations.Count > 0 ? _shownOperations[_selectedIndex] : null;

        if (_selectedOperation is not null)
        {
            var fileListContent = _treeEnabled
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
                ? TagDataPrinter.PrintComparison(
                    _selectedOperation.OriginalTagData,
                    _selectedOperation.TagData
                )
                : TagDataPrinter.PrintTagData(console, _selectedOperation.TagData);

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

        layout[HeaderLayoutKey].Update(new Text("Help will be here!"));

        console.Write(layout);

        var actionRequest = userActionReader.Read();
        switch (actionRequest.ActionName)
        {
            case ActionMoveDown:
                _selectedIndex++;
                break;
            case ActionMoveUp:
                _selectedIndex--;
                break;
            case ActionToggleTree:
                ToggleTree();
                break;
            case ActionToggleFilter:
                ToggleFilter();
                break;
            case ActionQuit:
            case "q":
                _running = false;
                break;
            case "write":
            case "w":
                Write();
                break;
            case "writeall":
            case "wa":
                WriteAll(_shownOperations);
                _treeListFactory.BuildTreeLines(_shownOperations);
                break;
            default:
                await DispatchAction(actionRequest);
                break;
        }
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
                    new FileWithTagData(_selectedOperation.Path, _selectedOperation.TagData),
                    _shownOperations.Select(x => new FileWithTagData(x.Path, x.TagData)).ToList(),
                    DispatchType.BeforeProcess
                );

                await actionDispatcher.Dispatch(
                    actionRequest,
                    new FileWithTagData(_selectedOperation.Path, _selectedOperation.TagData),
                    _shownOperations.Select(x => new FileWithTagData(x.Path, x.TagData)).ToList(),
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
                var allFiles = operations
                    .Select(x => new FileWithTagData(x.Path, x.TagData))
                    .ToList();
                await Parallel.ForEachAsync(
                    operations,
                    async (operation, _) =>
                    {
                        try
                        {
                            await actionDispatcher.Dispatch(
                                action,
                                new FileWithTagData(operation.Path, operation.TagData),
                                allFiles,
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
        _selectedOperation.Write(tagger);
        _treeListFactory.BuildTreeLines(_shownOperations);
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
                    operation.Write(tagger);
                    task.Description =
                        $"Writing metadata {i + 1} of {operationsToWrite.Count}({operation.Path.EscapeMarkup()})";
                    task.Increment(1);
                }
            });
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
                operations[itemIndex].Path
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
}
