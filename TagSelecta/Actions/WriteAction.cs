using TagSelecta.Actions.Base;
using TagSelecta.BaseCommands;
using TagSelecta.Tagging;
using Spectre.Console.Cli;
using TagSelecta.Print;

namespace TagSelecta.Actions;

public class WriteSettings : FileSettings
{
    [CommandOption("--genre|-g")]
    public string[]? Genre { get; set; }

    [CommandOption("--artist|-a")]
    public string[]? Artist { get; set; }

    [CommandOption("--albumartist|-A")]
    public string[]? AlbumArtist { get; set; }

    [CommandOption("--title|-t")]
    public string? Title { get; set; }

    [CommandOption("--album|-l")]
    public string? Album { get; set; }

    [CommandOption("--year|-y")]
    public uint? Year { get; set; }

    [CommandOption("--track|-n")]
    public uint? Track { get; set; }

    [CommandOption("--tracktotal|-N")]
    public uint? TrackTotal { get; set; }

    [CommandOption("--comment|-c")]
    public string? Comment { get; set; }

    [CommandOption("--disc|-d")]
    public uint? Disc { get; set; }

    [CommandOption("--disctotal|-D")]
    public uint? DiscTotal { get; set; }

    [CommandOption("--label|-L")]
    public string? Label { get; set; }

    [CommandOption("--catalogno|-C")]
    public string? CatalogNumber { get; set; }
}

public class WriteAction : IAction<WriteSettings>
{
    public void Execute(ActionContext<WriteSettings> context)
    {
        var tags = Tagger.ReadTags(context.File);
        tags.Genre = UpdateList(context.Settings.Genre, tags.Genre);
        tags.Artist = UpdateList(context.Settings.Artist, tags.Artist);
        tags.AlbumArtist = UpdateList(context.Settings.AlbumArtist, tags.AlbumArtist);
        tags.Title = UpdateString(context.Settings.Title, tags.Title);
        tags.Album = UpdateString(context.Settings.Album, tags.Album);
        tags.Year = UpdateInt(context.Settings.Year, tags.Year);
        tags.Label = UpdateString(context.Settings.Label, tags.Label);
        tags.CatalogNumber = UpdateString(context.Settings.CatalogNumber, tags.CatalogNumber);
        tags.Track = UpdateInt(context.Settings.Track, tags.Track);
        tags.TrackTotal = UpdateInt(context.Settings.TrackTotal, tags.TrackTotal);
        tags.Disc = UpdateInt(context.Settings.Disc, tags.Disc);
        tags.DiscTotal = UpdateInt(context.Settings.DiscTotal, tags.DiscTotal);
        tags.Comment = context.Settings.Comment ?? tags.Comment;
        context.Console.PrintTagData(tags);
        if (context.ConfirmPrompt())
        {
            Tagger.WriteTags(context.File, tags);
            return;
        }
    }

    private static string UpdateString(string? newVal, string oldVal)
    {
        if (newVal != null)
        {
            return newVal.Trim();
        }
        return oldVal;
    }

    private static uint UpdateInt(uint? newVal, uint oldVal)
    {
        if (newVal != null)
        {
            return newVal.Value;
        }
        return oldVal;
    }

    private static List<string> UpdateList(IEnumerable<string>? newList, List<string> currentList)
    {
        if (newList != null)
        {
            return [.. newList.Where(x => !string.IsNullOrEmpty(x))];
        }
        return currentList;
    }
}
