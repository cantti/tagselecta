using Spectre.Console;
using Spectre.Console.Rendering;

namespace TagSelecta.Commands.Tui.Widgets;

public class HelpWidget(Appearance theme, string topic = "") : Renderable
{
    private const string Fields =
        "album, albumartist, artist, bpm, comment, composer, conductor, copyright, date, discnumber, disctotal, genre, isrc, publisher, title, tracknumber, tracktotal";

    private static readonly HelpEntry[] Entries =
    [
        new(
            "help",
            "h",
            "Show help",
            ":help [topic]",
            "Show general help or help for a command or topic. Press esc to close help."
        ),
        new(
            "edit",
            "e",
            "Edit tags",
            ":edit artist=\"Artist 1\" title=\"Track Name\"",
            "Updates fields on selected files. Other fields are left unchanged. Use an empty value to clear a field. Separate multiple artists, album artists, composers or genres with semicolons. See :help templates for formatting and use :write to save changes."
        ),
        new(
            "clear",
            "",
            "Clear fields",
            ":clear title artist picture",
            "Clear the specified fields on selected files."
        ),
        new(
            "clearexcept",
            "",
            "Clear all fields except",
            ":clearexcept title artist picture",
            "Clear all fields except the specified fields on selected files."
        ),
        new(
            "autotrack",
            "",
            "Auto track number",
            ":autotrack",
            "Set track number and track total based on position in the directory. Disc number and disc total are removed unless keepdisk is specified."
        ),
        new(
            "split",
            "",
            "Split artists and other fields",
            ":split separator=\";\"",
            "Split artists, album artists, composers and genres. The separator option can be repeated."
        ),
        new(
            "titlecase",
            "",
            "Convert fields to title case",
            ":titlecase",
            "Convert field values to title case."
        ),
        new(
            "move",
            "mv",
            "Move files using a template",
            ":move template=\"../{{ date }} - {{ album }}/{{ filename }}.{{ ext }}\"",
            "Move or rename files using a template. Use keepemptydirs to keep empty source directories or donotmoveother to move only audio files. See :help templates for more examples."
        ),
        new(
            "extractpicture",
            "",
            "Extract picture",
            ":extractpicture",
            "Extract pictures from selected files. Picture files are written immediately, without :write."
        ),
        new(
            "discogs",
            "",
            "Download Discogs metadata",
            ":discogs release=\"https://www.discogs.com/release/...\"",
            "Download metadata from a Discogs release or master URL."
        ),
        new(
            "musicbrainz",
            "mb",
            "Download MusicBrainz metadata",
            ":musicbrainz release=\"https://musicbrainz.org/release/...\"",
            "Download metadata from a MusicBrainz release URL or ID."
        ),
        new(
            "selectdir",
            "",
            "Select all files in directory",
            ":selectdir",
            "Select all files in the focused file's directory."
        ),
        new(
            "selectall",
            "",
            "Select all files",
            ":selectall",
            "Select all files, including files hidden by the filter."
        ),
        new(
            "undo",
            "",
            "Undo changes",
            ":undo",
            "Undo changes to selected files, or the focused file if none are selected."
        ),
        new("quit", "q", "Quit", ":quit", "Exit the TUI."),
        new(
            "version",
            "",
            "Show version",
            ":version",
            "Show the current and latest available version."
        ),
        new("write", "w", "Save changes", ":write", "Write all pending changes to files."),
        new(
            "open",
            "o",
            "Open files or directories",
            ":open <path>",
            "Open files or directories, replacing the current file list."
        ),
        new(
            "macro",
            "m",
            "Insert a macro into the prompt",
            ":macro <name>",
            "Insert the named macro into the command prompt."
        ),
        new("run", "r", "Run a macro", ":run <name>", "Run the named macro immediately."),
    ];

    private static readonly HelpEntry[] NavigationEntries =
    [
        Navigation("select", "Toggle selection of the focused file and move down."),
        Navigation("escape", "Close help or pictures; otherwise, clear selection."),
        Navigation("moveup", "Move focus up."),
        Navigation("movedown", "Move focus down."),
        Navigation("movestart", "Move focus to the first file."),
        Navigation("moveend", "Move focus to the last file."),
        Navigation("metadataup", "Scroll metadata up."),
        Navigation("metadatadown", "Scroll metadata down."),
        Navigation("togglefilelist", "Toggle the file list."),
        Navigation("togglemetadata", "Toggle the metadata panel."),
        Navigation("toggletree", "Toggle the tree view."),
        Navigation("togglefilter", "Toggle the filter for changed files."),
        Navigation("togglepicture", "Toggle the picture view."),
        Navigation(
            "openpicture",
            "Open a picture from the focused file in the default image viewer."
        ),
        Navigation("togglekeymaphelp", "Show keymap help."),
        Navigation("togglecommandhelp", "Show general command help."),
        Navigation("opendocs", "Open the documentation in a browser."),
    ];

    public static IEnumerable<string> Topics =>
        Entries
            .Concat(NavigationEntries)
            .Select(x => x.Name)
            .Concat(["keymap", "templates", "template"]);

    private static HelpEntry Navigation(string name, string details)
    {
        return new HelpEntry(name, "", details, $":{name}", details);
    }

    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        IRenderable content;
        var title = "Help";
        if (string.IsNullOrWhiteSpace(topic))
        {
            var grid = new Grid();
            grid.AddColumns(2);
            foreach (var pair in Entries.Chunk(2))
            {
                grid.AddRow(
                    pair.Select(entry =>
                            $"[bold {theme.AccentFg}]:{entry.Name}[/] [{theme.TextFg}]{Markup.Escape(entry.Summary)}[/]"
                        )
                        .Concat(pair.Length == 1 ? [""] : [])
                        .ToArray()
                );
            }

            content = new Rows(
                ThemeText(
                    "Use :help <command>, :help templates or :help keymap. Press esc to close help."
                ),
                Text.Empty,
                grid
            );
        }
        else if (topic.Equals("keymap", StringComparison.OrdinalIgnoreCase))
        {
            title = "Keymap help";
            content = KeymapHelp();
        }
        else if (
            topic.Equals("templates", StringComparison.OrdinalIgnoreCase)
            || topic.Equals("template", StringComparison.OrdinalIgnoreCase)
        )
        {
            title = "Help: templates";
            content = new Rows(
                ThemeText("Scriban templates can be used in :edit values and :move template=."),
                ThemeText(
                    "Use {{ field }} for tags. filename, ext and path come from the file; year comes from date."
                ),
                ThemeText(
                    "Quote values containing spaces. pad(tracknumber) pads a track number with zeros."
                ),
                Text.Empty,
                ThemeText(":edit title=\"{{ filename }}\""),
                ThemeText(":edit genre=\"{{ genre | string.downcase }}\""),
                ThemeText(
                    ":move template=\"../{{ year }} - {{ album }}/{{ pad(tracknumber) }}. {{ title }}.{{ ext }}\""
                ),
                Text.Empty,
                ThemeText("Changes to tags and paths are saved with :write.")
            );
        }
        else
        {
            var entry = Entries
                .Concat(NavigationEntries)
                .FirstOrDefault(x =>
                    x.Name.Equals(topic, StringComparison.OrdinalIgnoreCase)
                    || (
                        x.Alias.Length > 0
                        && x.Alias.Equals(topic, StringComparison.OrdinalIgnoreCase)
                    )
                );
            if (entry is null)
            {
                content = ThemeText(
                    $"Unknown help topic: {topic}. Use :help to see available topics."
                );
            }
            else
            {
                title = $"Help: {entry.Name}";
                var alias = entry.Alias.Length > 0 ? $"Alias: :{entry.Alias}" : "";
                var rows = new List<IRenderable>
                {
                    new Markup($"[bold {theme.AccentFg}]{Markup.Escape(entry.Usage)}[/]"),
                    ThemeText(alias),
                    Text.Empty,
                    ThemeText(entry.Details),
                };
                if (entry.Name == "edit")
                {
                    rows.Add(Text.Empty);
                    rows.Add(ThemeText("Standard fields:"));
                    rows.Add(ThemeText(Fields));
                }

                content = new Rows(rows);
            }
        }

        return SectionPanel
            .Create($"[{theme.TextFg}]{Markup.Escape(title)}[/]", content, theme.Border)
            .ToSegments(options, maxWidth);
    }

    private Text ThemeText(string text)
    {
        return new Text(text, new Style(Color.FromHex(theme.TextFg)));
    }

    private IRenderable KeymapHelp()
    {
        var keys = new List<(string Key, string AltKey, string Action)>
        {
            (":", "", "Command mode"),
            ("space", "tab", "Select"),
            ("escape", "", "Close help or picture; otherwise, clear selection"),
            ("a", "*", "Select all"),
            ("Shift+A", "", "Select all files in directory"),
            ("j", "down", "Move down"),
            ("k", "up", "Move up"),
            ("Shift+J", "", "Scroll metadata down"),
            ("Shift+K", "", "Scroll metadata up"),
            ("e", "", "Toggle file explorer"),
            ("m", "", "Toggle metadata panel"),
            ("t", "", "Toggle tree"),
            ("f", "", "Toggle filter"),
            ("p", "", "Toggle picture"),
            ("Shift+P", "", "Open picture in default image viewer"),
            ("u", "", "Undo"),
            ("g", "", "Go to start"),
            ("Shift+G", "", "Go to end"),
            ("?", "", "Show keymap help"),
            ("h", "", "Show command help"),
            ("Shift+H", "", "Open docs in browser"),
            ("q", "", "Quit"),
        };
        var grid = new Grid();
        grid.AddColumns(3);
        foreach (var key in keys)
        {
            grid.AddRow(
                $"[bold {theme.AccentFg}]{Markup.Escape(key.Key)}[/]",
                $"[bold {theme.AccentFg}]{Markup.Escape(key.AltKey)}[/]",
                $"[{theme.TextFg}]{Markup.Escape(key.Action)}[/]"
            );
        }

        return grid;
    }

    private record HelpEntry(
        string Name,
        string Alias,
        string Summary,
        string Usage,
        string Details
    );
}
