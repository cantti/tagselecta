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

    protected override bool BeforeProcessTagData(ITagDataActionContext<SplitSettings> context)
    {
        if (context.Settings.Separator is not null)
        {
            separators = context.Settings.Separator;
        }
        return true;
    }

    protected override void ProcessTagData(ITagDataActionContext<SplitSettings> context)
    {
        var artists = context.TagData.Artist.SelectMany(Split).Distinct().ToList();
        var albumArtists = context.TagData.AlbumArtist.SelectMany(Split).Distinct().ToList();
        var composers = context
            .TagData.Composer.Select(Split)
            .SelectMany(x => x)
            .Distinct()
            .ToList();

        context.TagData.Artist = artists;
        context.TagData.AlbumArtist = albumArtists;
        context.TagData.Composer = composers;
    }

    private List<string> Split(string input)
    {
        return input
            .Split(separators, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .ToList();
    }
}
