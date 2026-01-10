using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Cli.Tui.Widgets;

public class StatusWidget(string message) : Renderable
{
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var text = message;
        return ((IRenderable)new Markup(text)).Render(options, maxWidth);
    }
}
