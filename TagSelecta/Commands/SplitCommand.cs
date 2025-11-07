using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.BaseCommands;
using TagSelecta.Tagging;

namespace TagSelecta.Commands;

public class SplitSettings : FileSettings
{
    [CommandOption("--separator|-s")]
    // last space is reauired otherwise . deleted
    [Description("Default values are: , ; feat. ")]
    public string[]? Separator { get; set; }
}

public class SplitCommand(IAnsiConsole console) : FileCommand<SplitSettings>(console)
{
    private string[] separators = [",", ";", "feat."];

    protected override void BeforeExecute()
    {
        if (Settings.Separator is not null)
        {
            separators = Settings.Separator;
        }
    }

    protected override void Execute(string file, int index)
    {
        var originalTags = Tagger.ReadTags(file);
        var tags = originalTags.Clone();
        var artists = tags.Artists.SelectMany(Split).Distinct().ToList();
        var albumArtists = tags.AlbumArtists.SelectMany(Split).Distinct().ToList();
        var composers = tags.Composers.Select(Split).SelectMany(x => x).Distinct().ToList();

        tags.Artists = artists;
        tags.AlbumArtists = albumArtists;
        tags.Composers = composers;

        if (!TagDataChanged(originalTags, tags))
        {
            Skip();
            return;
        }

        if (ConfirmPrompt())
        {
            Tagger.WriteTags(file, tags);
        }
    }

    private List<string> Split(string input)
    {
        return
        [
            .. input.Split(separators, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()),
        ];
    }
}
