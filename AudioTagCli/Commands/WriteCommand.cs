using AudioTagCli.BaseCommands;
using AudioTagCli.Misc;
using AudioTagCli.Tagging;
using Spectre.Console;
using Spectre.Console.Cli;

namespace AudioTagCli.Commands;

public class WriteSettings : FileProcessingSettings
{
    [CommandOption("--genre|-g")]
    public string[]? Genre { get; set; }

    [CommandOption("--artist|-a")]
    public string? Artist { get; set; }

    [CommandOption("--albumartist|-A")]
    public string? AlbumArtist { get; set; }

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

public class WriteCommand(IAnsiConsole console) : FileProcessingCommandBase<WriteSettings>(console)
{
    protected override async Task ProcessFileAsync(
        StatusContext ctx,
        WriteSettings settings,
        string[] files,
        string file
    )
    {
        var tags = Tagger.ReadTags(file);
        tags.Genre = UpdateList(settings.Genre, tags.Genre);
        tags.Artist = settings.Artist is not null ? GetMultiValue(settings.Artist) : tags.Artist;
        tags.AlbumArtist = settings.AlbumArtist is not null
            ? GetMultiValue(settings.AlbumArtist)
            : tags.AlbumArtist;
        tags.Title = UpdateString(settings.Title, tags.Title);
        tags.Album = UpdateString(settings.Album, tags.Album);
        tags.Year = UpdateInt(settings.Year, tags.Year);
        tags.Label = UpdateString(settings.Label, tags.Label);
        tags.CatalogNumber = UpdateString(settings.CatalogNumber, tags.CatalogNumber);
        tags.Track = UpdateInt(settings.Track, tags.Track);
        tags.TrackTotal = UpdateInt(settings.TrackTotal, tags.TrackTotal);
        tags.Disc = UpdateInt(settings.Disc, tags.Disc);
        tags.DiscTotal = UpdateInt(settings.DiscTotal, tags.DiscTotal);
        tags.Comment = settings.Comment ?? tags.Comment;
        ctx.Status("Writing tags...");
        Tagger.WriteTags(file, tags);
        await Task.CompletedTask;
    }

    private static string? UpdateString(string? newVal, string? oldVal)
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

    private static List<string> GetMultiValue(string value)
    {
        return
        [
            .. value.Split(
                ';',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
            ),
        ];
    }
}
