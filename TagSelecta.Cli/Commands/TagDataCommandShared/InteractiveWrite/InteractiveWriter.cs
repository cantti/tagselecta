using Spectre.Console;
using Spectre.Console.Rendering;
using TagSelecta.Cli.Commands.TagDataCommandShared.BulkWrite;
using TagSelecta.Tagging;

namespace TagSelecta.Cli.Commands.TagDataCommandShared.InteractiveWrite;

public class InteractiveWriter(
    IAnsiConsole console,
    IUserActionReader userActionReader,
    ITagger tagger,
    IBulkWriter bulkWriter
) : IInteractiveWriter
{
    private const string HeaderLayoutKey = "navigation";
    private const string FilesLayoutKey = "files";
    private const string TagDataLayoutKey = "body";

    private int _selectedIndex = 0;
    private bool _filterEnabled = false;
    private bool _treeEnabled = false;

    public void Start(List<TagDataOperation> operations)
    {
        console.Cursor.Hide();

        var filtered = operations;

        var treeListFactory = new TreeFileListFactory();
        treeListFactory.BuildTreeLines(filtered);

        while (true)
        {
            console.Clear();

            var navigation = userActionReader.RenderNavigation();

            var filesContentSize = Math.Min(
                (Console.WindowHeight - navigation.Size) / 2,
                // +2 for navigation and add empty row
                filtered.Count + 2
            );

            var layout = new Layout("root").SplitRows(
                new Layout(HeaderLayoutKey).Size(navigation.Size).Update(Text.Empty),
                new Layout(FilesLayoutKey).Size(filesContentSize).Update(Text.Empty),
                new Layout(TagDataLayoutKey).Update(Text.Empty)
            );

            _selectedIndex = Math.Clamp(_selectedIndex, 0, Math.Max(0, filtered.Count - 1));

            var selectedOperation = filtered.Count > 0 ? filtered[_selectedIndex] : null;

            if (selectedOperation is not null)
            {
                var fileListContent = _treeEnabled
                    ? treeListFactory.Render(
                        filtered,
                        _selectedIndex,
                        filesContentSize - 2,
                        _filterEnabled
                    )
                    : RenderFileList(
                        filtered,
                        _selectedIndex,
                        filesContentSize - 2,
                        _filterEnabled
                    );

                layout[FilesLayoutKey].Update(fileListContent);

                var tagDataRenderable = selectedOperation.HasChanges
                    ? TagDataPrinter.PrintComparison(
                        selectedOperation.OriginalTagData,
                        selectedOperation.TagData
                    )
                    : TagDataPrinter.PrintTagData(console, selectedOperation.TagData);

                if (selectedOperation.Exception is null)
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
                                    $"Error: {selectedOperation.Exception.Message}",
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

            layout[HeaderLayoutKey].Update(navigation.Content);

            console.Write(layout);

            var cmd = userActionReader.Read();
            if (cmd == UserAction.Next)
            {
                _selectedIndex++;
            }
            else if (cmd == UserAction.Previous)
            {
                _selectedIndex--;
            }
            else if (cmd == UserAction.WriteAll)
            {
                bulkWriter.WriteAll(operations);
                treeListFactory.BuildTreeLines(operations);
            }
            else if (cmd == UserAction.Write)
            {
                if (
                    selectedOperation is
                    { Status: TagDataOperationStatus.Pending, HasChanges: true }
                )
                {
                    selectedOperation.Write(tagger);
                    treeListFactory.BuildTreeLines(operations);
                }
            }
            else if (cmd == UserAction.ToggleFilter)
            {
                _filterEnabled = !_filterEnabled;
                filtered = _filterEnabled
                    ? operations.Where(x => x.HasChanges).ToList()
                    : operations;
                treeListFactory.BuildTreeLines(filtered);
                _selectedIndex = 0;
            }
            else if (cmd == UserAction.ToggleTree)
            {
                _treeEnabled = !_treeEnabled;
            }
            else
            {
                break;
            }
        }
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
}
