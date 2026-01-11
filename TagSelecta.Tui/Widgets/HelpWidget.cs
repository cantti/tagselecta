using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Tui.Widgets;

public class HelpWidget : Renderable
{
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        IRenderable content;

        var keys = new List<(string Key, string Action)>
        {
            ("space, tab", "Select"),
            ("escape", "Clear selection"),
            ("a, *", "Select All"),
            ("j, move down", "Move down"),
            ("k, move up", "Move up"),
            ("t", "Toggle tree"),
            ("f", "Toggle filter"),
            ("u", "Undo. Only if not written!"),
            (":selectdir", "Select all files in dir"),
            (":write, :w", "Write"),
            (":edit, :e", "Edit tags (artist=Bach title=\"The Goldberg Variations\" all)"),
            (":autotrack", "Auto track number"),
            (":split", "Split artists"),
            (":titlecase", "Title case conversion"),
            (":extractpicture", "Extract picture"),
            (
                ":discogs",
                "release=https://www.discogs.com/master/163206-King-Tubby-Presents-The-Roots-Of-Dub"
            ),
            ("https://github.com/cantti/tagselecta", "Read more"),
        };
        var grid = new Grid();
        grid.AddColumn();
        grid.AddColumn();
        foreach (var key in keys)
        {
            grid.AddRow($"[bold blue]{key.Key}[/]", key.Action);
        }
        content = new Rows(new Text("Help:", new Style(Color.Yellow)), grid);
        return content.Render(options, maxWidth);
    }
}
