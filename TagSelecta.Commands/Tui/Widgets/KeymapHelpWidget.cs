using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Commands.Tui.Widgets;

public class KeymapHelpWidget : Renderable
{
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var keys = new List<(string Key, string Action)>
        {
            ("space, tab", "Select"),
            ("escape", "Clear selection"),
            ("a, *", "Select All"),
            ("A", "Select all files in directory"),
            ("j, move down", "Move down"),
            ("k, move up", "Move up"),
            ("t", "Toggle tree"),
            ("f", "Toggle filter"),
            ("u", "Undo"),
            ("g", "Go to start"),
            ("G", "Go to end"),
        };
        var grid = new Grid();
        grid.AddColumns(2);
        foreach (var key in keys)
        {
            grid.AddRow($"[bold blue]{Markup.Escape(key.Key)}[/]", key.Action);
        }

        IRenderable content = new Rows(new Text("Keymap help:", new Style(Color.Yellow)), grid);
        return content.Render(options, maxWidth);
    }
}
