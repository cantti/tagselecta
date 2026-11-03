using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Commands.Tui.Widgets;

public class StatusWidget(string message) : Renderable
{
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var text = message;
        return new Text(text).ToSegments(options, maxWidth);
    }
}
