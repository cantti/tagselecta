using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Commands.Tui.Widgets;

public class FileListWidget(
    TuiAppTheme theme,
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

        if (linesToPrint < 0)
        {
            linesToPrint = 0;
        }

        var items = new List<IRenderable>();
        var contentWidth = ExplorerPanel.GetContentWidth(maxWidth);

        for (var i = 0; i < linesToPrint; i++)
        {
            var itemIndex = windowStart + i;
            var path = filesList[itemIndex].BackupPath;

            var cols = new List<IRenderable>();
            var fg = filesList[itemIndex].HasChanges
                ? Color.FromHex(theme.DangerFg)
                : Color.FromHex(theme.TextFg);
            var bg = focusedIndex == itemIndex ? Color.FromHex(theme.FocusedBg) : Color.Default;
            var selectedMarker = ExplorerPanel.GetSelectionMarker(
                filesList[itemIndex].IsSelected,
                theme.SelectionSymbol
            );
            cols.Add(new Text(selectedMarker, new Style(fg, bg)));
            var markerWidth = new Segment(selectedMarker).CellCount();
            var pathSegment = new Segment(path);
            var pathWidth = pathSegment.CellCount();
            if (pathWidth + markerWidth > contentWidth)
            {
                var start = Math.Clamp(pathWidth + markerWidth + 3 - contentWidth, 0, pathWidth);
                cols.Add(new Text("...", new Style(fg, bg)));
                path = pathSegment.Split(start).Second?.Text ?? "";
            }

            cols.Add(new Text(path, new Style(fg, bg)));
            items.Add(new Columns(cols) { Expand = false, Padding = new Padding(0) });
        }

        return ExplorerPanel
            .Create(filesList.Count, new Rows(items), theme)
            .ToSegments(options, maxWidth);
    }
}
