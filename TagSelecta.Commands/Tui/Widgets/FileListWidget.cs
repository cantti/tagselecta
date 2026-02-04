using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Commands.Tui.Widgets;

public class FileListWidget(
    IEnumerable<TagDataActionTarget> files,
    TagDataActionTarget? focusedFile,
    int windowSize
) : Renderable
{
    private readonly List<TagDataActionTarget> _files = files.ToList();

    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var filesList = _files.ToList();

        // center around the current index (5 lines above, 4 below), but keep a full window when possible
        var focusedIndex = filesList.FindIndex(x => x == focusedFile);
        var windowStart = focusedIndex - windowSize / 2;

        // clamp so we don't go before 0 or past the last possible full window start
        var maxStart = Math.Max(0, filesList.Count - windowSize);
        windowStart = Math.Clamp(windowStart, 0, maxStart);

        var linesToPrint = Math.Min(windowSize, filesList.Count - windowStart);

        var items = new List<IRenderable>();

        for (var i = 0; i < linesToPrint; i++)
        {
            var itemIndex = windowStart + i;
            var path = filesList[itemIndex].BackupPath;

            var cols = new List<IRenderable>();
            var fg = filesList[itemIndex].HasChanges ? Color.Red : Color.Default;
            var bg = focusedIndex == itemIndex ? Color.Grey : Color.Default;
            var selectedMarker = filesList[itemIndex].IsSelected ? "[x] " : "[ ] ";
            cols.Add(new Text(selectedMarker, new Style(fg, bg)));
            if (path.Length + selectedMarker.Length > maxWidth)
            {
                var start = path.Length + selectedMarker.Length + 3 - maxWidth;
                cols.Add(new Text("...", new Style(fg, bg)));
                path = path.Substring(start);
            }

            cols.Add(new Text(path, new Style(fg, bg)));
            items.Add(new Columns(cols) { Expand = false, Padding = new Padding(0) });
        }

        IRenderable content = new Rows(
            new Text($"Files ({filesList.Count}):", new Style(Color.Yellow))
            {
                Overflow = Overflow.Ellipsis,
            },
            new Rows(items)
        );

        return content.Render(options, maxWidth);
    }
}
