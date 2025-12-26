using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Cli.Commands.TagDataCommands.Common;

namespace TagSelecta.Cli.Commands.TagDataCommands;

public class SplitSettings : BaseSettings
{
    [CommandOption("--separator|-s")]
    // last space is reauired otherwise . deleted
    [Description("Default values are: , ; feat. ")]
    public string[]? Separator { get; set; }
}

public class SplitAction : TagDataAction<SplitSettings>
{
    private string[] separators = [",", ";", "feat."];

    protected override bool BeforeProcessTagData(SplitSettings settings)
    {
        if (settings.Separator is not null)
        {
            separators = settings.Separator;
        }
        return true;
    }

    protected override void ProcessTagData(Item current, List<Item> items, SplitSettings settings)
    {
        var artists = current.TagData.Artist.SelectMany(Split).Distinct().ToList();
        var albumArtists = current.TagData.AlbumArtist.SelectMany(Split).Distinct().ToList();
        var composers = current
            .TagData.Composer.Select(Split)
            .SelectMany(x => x)
            .Distinct()
            .ToList();

        current.TagData.Artist = artists;
        current.TagData.AlbumArtist = albumArtists;
        current.TagData.Composer = composers;
    }

    private List<string> Split(string input)
    {
        return input
            .Split(separators, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .ToList();
    }
}
