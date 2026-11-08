using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Commands.Tui.Widgets;

public static class SectionPanel
{
    public const int HorizontalPadding = 4;
    public const int VerticalPadding = 2;

    public static Panel Create(string header, IRenderable content, string borderColor)
    {
        return new Panel(content).Header(header).Expand().BorderColor(Color.FromHex(borderColor));
    }
}
