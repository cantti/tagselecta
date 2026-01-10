using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Cli.Tui.Widgets;

public class StatusWidget(string message, bool inputBlocked) : Renderable
{
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var text = message;
        if (inputBlocked)
        {
            text = $"{text} Press [italic]c[/] to cancel.";
        }
        return ((IRenderable)new Markup(text)).Render(options, maxWidth);
    }
}
