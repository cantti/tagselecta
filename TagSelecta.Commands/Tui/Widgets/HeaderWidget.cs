using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Commands.Tui.Widgets;

public class HeaderWidget(TuiAppTheme theme) : Renderable
{
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        if (maxWidth <= 0)
        {
            return [];
        }

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

        var text = $"{keyLine}";

        text = text[..Math.Min(text.Length, maxWidth)].PadRight(maxWidth);

        return new Text(
            text,
            new Style(
                Color.FromHex(theme.HeaderLineFg),
                Color.FromHex(theme.HeaderLineBg),
                Decoration.Bold
            )
        ).ToSegments(options, maxWidth);
    }
}
