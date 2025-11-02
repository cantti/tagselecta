using System.ComponentModel;
using Spectre.Console.Cli;
using TagSelecta.Actions.Base;
using TagSelecta.BaseCommands;
using TagSelecta.Print;
using TagSelecta.Tagging;

namespace TagSelecta.Actions;

public class SplitSettings : FileSettings
{
    [CommandOption("--delimiters|-d")]
    // last space is reauired otherwise . deleted
    [Description("Default values are: , ; feat. ")]
    public string[]? Delimiters { get; set; }
}

public class SplitAction(Printer printer) : FileAction<SplitSettings>
{
    private string[] delimiters = [",", ";", "feat."];

    public override async Task BeforeExecute(ActionBeforeContext<SplitSettings> context)
    {
        if (context.Settings.Delimiters is not null)
        {
            delimiters = context.Settings.Delimiters;
        }
    }

    public override Task Execute(ActionContext<SplitSettings> context)
    {
        var tags = Tagger.ReadTags(context.File);
        var artist = tags.Artist.Select(Split).SelectMany(x => x).Distinct().ToList();
        var albumArtist = tags.AlbumArtist.Select(Split).SelectMany(x => x).Distinct().ToList();
        var composers = tags.Composers.Select(Split).SelectMany(x => x).Distinct().ToList();

        tags.Artist = artist;
        tags.AlbumArtist = albumArtist;
        tags.Composers = composers;

        printer.PrintTagData(tags);

        if (context.ConfirmPrompt())
        {
            Tagger.WriteTags(context.File, tags);
        }

        return Task.CompletedTask;
    }

    private List<string> Split(string input)
    {
        return
        [
            .. input.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()),
        ];
    }
}
