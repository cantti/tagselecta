using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Commands.Tui.Widgets;

public class HeaderWidget : Renderable
{
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var keys = new List<(string Key, string Action)>
        {
            ("q", "quit"),
            ("j/k", "move"),
            ("space", "select"),
            ("e", "file list"),
            ("p", "picture"),
            ("h", "command help"),
            ("?", "hotkeys"),
        };

        var keyLine = string.Join(", ", keys.Select(x => $"[{x.Key}] {x.Action}"));
        var headerLine = $"{keyLine}";

        var visibleHeaderLine = headerLine;
        if (maxWidth <= 0)
        {
            visibleHeaderLine = "";
        }
        else if (headerLine.Length > maxWidth)
        {
            visibleHeaderLine = headerLine[..maxWidth];
        }
        else
        {
            visibleHeaderLine = headerLine.PadRight(maxWidth);
        }

        IRenderable content = new Rows(
            new Text(
                visibleHeaderLine,
                new Style(Color.Default, Color.Default, Decoration.Invert | Decoration.Bold)
            )
        );
        return content.Render(options, maxWidth);
    }
}
