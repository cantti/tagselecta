using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Commands.Tui.Widgets;

public class CommandHelpWidget : Renderable
{
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        IRenderable content = new Rows(
            new SectionHeaderWidget("Command help:"),
            CommandHelp(),
            Text.Empty,
            EditableFieldsHelp()
        );
        return content.Render(options, maxWidth);
    }

    private static IRenderable CommandHelp()
    {
        var commandColor = Color.Blue;
        var commands = new List<(string Command, string Action)>
        {
            ($"[bold {commandColor}]:selectdir[/]", "Select all files in dir"),
            ($"[bold {commandColor}]:write[/]", "Save changes"),
            (
                $"[bold {commandColor}]:edit[/] artist=Bach title=\"The Goldberg Variations\" url=https://example.com",
                "Edit tags"
            ),
            ($"[bold {commandColor}]:autotrack[/]", "Auto track number"),
            ($"[bold {commandColor}]:split[/]", "Split artists"),
            ($"[bold {commandColor}]:titlecase[/]", "Title case conversion"),
            (
                $"[bold {commandColor}]:move[/] template=\"../{{{{ year }}}} - {{{{ album }}}}/{{{{ filename }}}}.{{{{ext}}}}\"",
                "Move file using template"
            ),
            ($"[bold {commandColor}]:extractpicture[/]", "Extract picture"),
            ($"[bold {commandColor}]:clearexcept[/] artist album title", "Clear all fields except"),
            (
                $"[bold {commandColor}]:discogs[/] release=\"https://www.discogs.com/release/...\"",
                "Discogs"
            ),
            (
                $"[bold {commandColor}]:musicbrainz[/] release=\"https://musicbrainz.org/release/...\"",
                "MusicBrainz"
            ),
            ($"[bold {commandColor}]:macro[/] <name>", "Execute macro <name>"),
        };
        var result = new Grid();
        result.AddColumns(2);
        foreach (var (command, action) in commands)
        {
            result.AddRow(command, action);
        }

        return result;
    }

    private static IRenderable EditableFieldsHelp()
    {
        var fields = new[]
        {
            "album",
            "albumartist",
            "artist",
            "bpm",
            "comment",
            "composer",
            "conductor",
            "copyright",
            "date",
            "discnumber",
            "disctotal",
            "genre",
            "isrc",
            "publisher",
            "title",
            "tracknumber",
            "tracktotal",
        };
        return new Markup($"[bold {Color.Blue}]Standard fields[/]: {string.Join(", ", fields)}");
    }
}
