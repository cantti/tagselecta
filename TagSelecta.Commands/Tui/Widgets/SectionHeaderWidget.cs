using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Commands.Tui.Widgets;

public class SectionHeaderWidget(string text) : Renderable
{
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        return (
            (IRenderable)new Text(text, new Style(Color.Yellow, Color.Default, Decoration.Bold))
        ).Render(options, maxWidth);
    }
}
