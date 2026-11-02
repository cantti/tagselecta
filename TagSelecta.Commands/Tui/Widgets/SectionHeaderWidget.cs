using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Commands.Tui.Widgets;

public class SectionHeaderWidget(string text) : Renderable
{
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        string visibleText = text[..Math.Min(text.Length, maxWidth)].PadRight(maxWidth);

        return [new Segment(visibleText, new Style(Color.Black, Color.Green, Decoration.Bold))];
    }
}
