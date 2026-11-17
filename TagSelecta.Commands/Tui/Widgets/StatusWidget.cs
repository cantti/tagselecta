using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Commands.Tui.Widgets;

public class StatusWidget(ColorScheme theme, string message) : Renderable
{
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var text = !string.IsNullOrEmpty(message) ? message : "[q] quit, [h] help";
        return SectionPanel
            .Create("Status", new Text(text, new Style(Color.FromHex(theme.TextFg))), theme.Border)
            .ToSegments(options, maxWidth);
    }
}
