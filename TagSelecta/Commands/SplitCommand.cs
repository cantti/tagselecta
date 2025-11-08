using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.BaseCommands;
using TagSelecta.Tagging;

namespace TagSelecta.Commands;

public class SplitSettings : BaseSettings
{
    [CommandOption("--separator|-s")]
    // last space is reauired otherwise . deleted
    [Description("Default values are: , ; feat. ")]
    public string[]? Separator { get; set; }
}

public class SplitCommand(IAnsiConsole console) : TagDataCommand<SplitSettings>(console)
{
    private string[] separators = [",", ";", "feat."];

    protected override void BeforeProcess()
    {
        if (Settings.Separator is not null)
        {
            separators = Settings.Separator;
        }
    }

    protected override void ProcessTagData()
    {
        var artists = TagData.Artists.SelectMany(Split).Distinct().ToList();
        var albumArtists = TagData.AlbumArtists.SelectMany(Split).Distinct().ToList();
        var composers = TagData.Composers.Select(Split).SelectMany(x => x).Distinct().ToList();

        TagData.Artists = artists;
        TagData.AlbumArtists = albumArtists;
        TagData.Composers = composers;
    }

    private List<string> Split(string input)
    {
        return
        [
            .. input.Split(separators, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()),
        ];
    }
}
