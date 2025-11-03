using System.ComponentModel;
using Spectre.Console.Cli;
using TagSelecta.Actions.Base;
using TagSelecta.BaseCommands;
using TagSelecta.Print;
using TagSelecta.Tagging;

namespace TagSelecta.Actions;

public class SplitSettings : FileSettings
{
    [CommandOption("--separator|-s")]
    // last space is reauired otherwise . deleted
    [Description("Default values are: , ; feat. ")]
    public string[]? Separator { get; set; }
}

public class SplitAction(Printer printer, ActionContext<SplitSettings> context)
    : IFileAction<SplitSettings>
{
    private string[] separators = [",", ";", "feat."];

    public async Task BeforeExecute()
    {
        if (context.Settings.Separator is not null)
        {
            separators = context.Settings.Separator;
        }
    }

    public Task Execute(string file, int index)
    {
        var tags = Tagger.ReadTags(file);
        var artist = tags.Artist.Select(Split).SelectMany(x => x).Distinct().ToList();
        var albumArtist = tags.AlbumArtist.Select(Split).SelectMany(x => x).Distinct().ToList();
        var composers = tags.Composers.Select(Split).SelectMany(x => x).Distinct().ToList();

        tags.Artist = artist;
        tags.AlbumArtist = albumArtist;
        tags.Composers = composers;

        printer.PrintTagData(tags);

        if (context.ConfirmPrompt())
        {
            Tagger.WriteTags(file, tags);
        }

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
