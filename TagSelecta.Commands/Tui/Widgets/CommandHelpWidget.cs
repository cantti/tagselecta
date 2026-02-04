using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Commands.Tui.Widgets;

public class CommandHelpWidget : Renderable
{
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        IRenderable content = new Rows(
            new Text("Command help:", new Style(Color.Yellow)),
            CommandHelp(),
            Text.Empty,
            EditableFieldsHelp(),
            Text.Empty,
            TemplateFieldsHelp()
        );
        return content.Render(options, maxWidth);
    }

    private static IRenderable CommandHelp()
    {
        var valueColor = Color.DarkOrange;
        var commandColor = Color.Blue;
        var parameterColor = Color.Magenta;
        var commands = new List<(string Command, string Action)>
        {
            ($"[bold {commandColor}]:selectdir[/]", "Select all files in dir"),
            ($"[bold {commandColor}]:write[/]", "Save changes"),
            (
                $"[bold {commandColor}]:edit[/] [{parameterColor}]artist[/]=[{valueColor}]Bach[/] [{parameterColor}]title[/]=[{valueColor}]\"The Goldberg Variations\"[/] [{parameterColor}]set[/]=[{valueColor}]custom_key=value[/]",
                "Edit tags"
            ),
            ($"[bold {commandColor}]:autotrack[/]", "Auto track number"),
            ($"[bold {commandColor}]:split[/]", "Split artists"),
            ($"[bold {commandColor}]:titlecase[/]", "Title case conversion"),
            (
                $"[bold {commandColor}]:move[/] [{parameterColor}]template[/]=[{valueColor}]\"../{{ year }} - {{ album }}/{{ filename }}.{{ext}}\"[/]",
                "Move file using template"
            ),
            ($"[bold {commandColor}]:extractpicture[/]", "Extract picture"),
            (
                $"[bold {commandColor}]:discogs[/] [{parameterColor}]url[/]=[{valueColor}]\"https://www.discogs.com/master/...\"[/]",
                "Discogs"
            ),
            ($"[bold {commandColor}]:macro[/] [{valueColor}]<name>[/]", "Execute macro"),
        };
        var result = new Grid();
        result.AddColumns(2);
        foreach (var (command, action) in commands)
            result.AddRow(command, action);
        return result;
    }

    private static IRenderable EditableFieldsHelp()
    {
        var fields = new[]
        {
            "artist(a)",
            "albumartist(A)",
            "album(l)",
            "title(t)",
            "track(n)",
            "tracktotal(N)",
            "disc(d)",
            "disctotal(D)",
            "date(y)",
            "genre(g)",
            "comment(c)",
            "composer(C)",
            "bpm",
            "catalognumber",
            "conductor",
            "copyright",
            "isrc",
            "label",
            "publisher",
            "picture(p)",
            "picturetype",
        };
        return new Markup($"[bold {Color.Blue}]Editable fields[/]: {string.Join(", ", fields)}");
    }

    private static IRenderable TemplateFieldsHelp()
    {
        var fields = new[]
        {
            "path",
            "filename",
            "ext",
            "album",
            "albumartist",
            "albumartists",
            "artist",
            "artists",
            "bpm",
            "catalognumber",
            "comment",
            "composer",
            "composers",
            "conductor",
            "copyright",
            "date",
            "disc",
            "disc00",
            "disctotal",
            "genre",
            "genres",
            "isrc",
            "label",
            "publisher",
            "title",
            "track",
            "track00",
            "track000",
            "tracktotal",
            "year",
            "extra.key1",
        };
        return new Markup($"[bold {Color.Blue}]Template fields[/]: {string.Join(", ", fields)}");
    }
}
