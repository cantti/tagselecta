using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace TagSelecta.TagDataActions;

public class SplitSettings : BaseSettings
{
    [CommandOption("--separator|-s")]
    // last space is reauired otherwise . deleted
    [Description("Default values are: , ; feat. ")]
    public string[]? Separator { get; set; }
}

public class SplitAction : ITagDataAction<SplitSettings>
{
    private string[] separators = [",", ";", "feat."];

    public Task<bool> BeforeProcessTagData(TagDataActionContext<SplitSettings> context)
    {
        if (context.Settings.Separator is not null)
        {
            separators = context.Settings.Separator;
        }
        return Task.FromResult(true);
    }

    public Task ProcessTagData(TagDataActionContext<SplitSettings> context)
    {
        var artists = context.TagData.Artists.SelectMany(Split).Distinct().ToList();
        var albumArtists = context.TagData.AlbumArtists.SelectMany(Split).Distinct().ToList();
        var composers = context
            .TagData.Composers.Select(Split)
            .SelectMany(x => x)
            .Distinct()
            .ToList();

        context.TagData.Artists = artists;
        context.TagData.AlbumArtists = albumArtists;
        context.TagData.Composers = composers;
        return Task.CompletedTask;
    }

    private List<string> Split(string input)
    {
        return
        [
            .. input.Split(separators, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()),
        ];
    }
}
