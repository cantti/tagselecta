using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Commands.Tui.Widgets;

public class SectionHeaderWidget(string text) : Renderable
{
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        string visibleText;
        if (text.Length > maxWidth)
        {
            visibleText = text[..maxWidth];
        }
        else
        {
            visibleText = text.PadRight(maxWidth);
        }

        return (
            (IRenderable)
                new Text(
                    visibleText,
                    new Style(Color.Yellow, Color.Default, Decoration.Invert | Decoration.Bold)
                )
        ).Render(options, maxWidth);
    }
}
