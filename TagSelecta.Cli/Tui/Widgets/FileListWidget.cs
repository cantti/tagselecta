using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Cli.Tui.Widgets;

public class FileListWidget(List<TagDataOperation> operations, int selectedIndex, int windowSize)
    : Renderable
{
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        IRenderable content;

        if (operations.Count == 0)
        {
            content = Text.Empty;
            return content.Render(options, maxWidth);
        }

        // center around the current index (5 lines above, 4 below), but keep a full window when possible
        var windowStart = selectedIndex - (windowSize / 2);

        // clamp so we don't go before 0 or past the last possible full window start
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

        content = new Rows(
            new Text($"Files ({operations.Count}):", new Style(Color.Yellow)),
            new Rows(items)
        );

        return content.Render(options, maxWidth);
    }
}
