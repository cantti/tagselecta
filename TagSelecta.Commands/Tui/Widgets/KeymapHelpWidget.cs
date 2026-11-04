using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Commands.Tui.Widgets;

public class KeymapHelpWidget(TuiAppTheme theme) : Renderable
{
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var keys = new List<(string Key, string AltKey, string Action)>
        {
            (":", "", "Command mode"),
            ("space", "tab", "Select"),
            ("escape", "", "Clear selection"),
            ("a", "*", "Select all"),
            ("A", "", "Select all files in directory"),
            ("j", "down", "Move down"),
            ("k", "up", "Move up"),
            ("e", "", "Toggle file explorer"),
            ("t", "", "Toggle tree"),
            ("f", "", "Toggle filter"),
            ("p", "", "Toggle picture"),
            ("P", "", "Open picture in default image viewer"),
            ("u", "", "Undo"),
            ("g", "", "Go to start"),
            ("G", "", "Go to end"),
            ("?", "", "Toggle this help"),
            ("h", "", "Toggle command help"),
            ("H", "", "Open docs in browser"),
            ("q", "", "Quit"),
        };
        var grid = new Grid();
        grid.AddColumns(3);
        foreach (var key in keys)
        {
            grid.AddRow(
                $"[bold {theme.AccentFg}]{Markup.Escape(key.Key)}[/]",
                $"[bold {theme.AccentFg}]{Markup.Escape(key.AltKey)}[/]",
                key.Action
            );
        }

        IRenderable content = new Rows(new SectionHeaderWidget(theme, "Keymap help:"), grid);
        return content.Render(options, maxWidth);
    }
}
