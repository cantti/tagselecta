using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Commands.Tui.Widgets;

public class SectionHeaderWidget(TuiAppTheme theme, string text) : Renderable
{
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        string visibleText = text[..Math.Min(text.Length, maxWidth)].PadRight(maxWidth);

        return new Text(
            visibleText,
            new Style(
                Color.FromHex(theme.HeaderLineFg),
                Color.FromHex(theme.HeaderLineBg),
                Decoration.Bold
            )
        ).ToSegments(options, maxWidth);
    }
}
