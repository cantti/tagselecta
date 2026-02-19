using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Commands.Tui.Widgets;

public class KeymapHelpWidget : Renderable
{
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var keys = new List<(string Key, string AltKey, string Action)>
        {
            ("space", "tab", "Select"),
            ("escape", "", "Clear selection"),
            ("a", "*", "Select all"),
            ("A", "", "Select all files in directory"),
            ("j", "down", "Move down"),
            ("k", "up", "Move up"),
            ("e", "", "Toggle file list (explorer)"),
            ("t", "", "Toggle tree"),
            ("f", "", "Toggle filter"),
            ("u", "", "Undo"),
            ("g", "", "Go to start"),
            ("G", "", "Go to end"),
            ("?", "", "Toggle this help"),
            ("h", "", "Toggle command help"),
            ("q", "", "Quit"),
        };
        var grid = new Grid();
        grid.AddColumns(3);
        foreach (var key in keys)
        {
            grid.AddRow(
                $"[bold blue]{Markup.Escape(key.Key)}[/]",
                $"[bold blue]{Markup.Escape(key.AltKey)}[/]",
                key.Action
            );
        }

        IRenderable content = new Rows(new SectionHeaderWidget("Keymap help:"), grid);
        return content.Render(options, maxWidth);
    }
}
