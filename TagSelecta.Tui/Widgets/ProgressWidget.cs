using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Tui.Widgets;

public class ProgressWidget : Renderable
{
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        IRenderable content;

        content = new Text("Processing...");

        return content.Render(options, maxWidth);
    }
}
