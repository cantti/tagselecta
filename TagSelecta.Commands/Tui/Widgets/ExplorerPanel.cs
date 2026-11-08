using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Commands.Tui.Widgets;

internal static class ExplorerPanel
{
    private const int BorderWidth = 2;
    private static readonly Padding ContentPadding = new(0);

    public static int GetContentWidth(int maxWidth)
    {
        return Math.Max(0, maxWidth - BorderWidth - ContentPadding.Left - ContentPadding.Right);
    }

    public static string GetSelectionMarker(bool selected, string symbol)
    {
        return selected ? $" {symbol} " : new string(' ', new Segment(symbol).CellCount() + 2);
    }

    public static Panel Create(int fileCount, IRenderable content, TuiAppTheme theme)
    {
        var panel = SectionPanel.Create($"Explorer ({fileCount})", content, theme.Border);
        panel.Padding = ContentPadding;
        return panel;
    }
}
