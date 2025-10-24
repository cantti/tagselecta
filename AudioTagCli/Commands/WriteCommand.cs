using AudioTagCli.BaseCommands;
using AudioTagCli.Misc;
using AudioTagCli.Tagging;
using Spectre.Console;
using Spectre.Console.Cli;

namespace AudioTagCli.Commands;

public class WriteSettings : FileProcessingSettings
{
    [CommandOption("--genre|-g")]
    public string? Genre { get; set; }

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

    [CommandOption("--label|-L")]
    public string? Label { get; set; }

    [CommandOption("--catalogno|-c")]
    public string? CatalogNumber { get; set; }

    [CommandOption("--track|-T")]
    public uint? Track { get; set; }
}

public class WriteCommand(IAnsiConsole console) : FileProcessingCommandBase<WriteSettings>(console)
{
    protected override async Task ProcessFileAsync(
        StatusContext ctx,
        WriteSettings settings,
        string file
    )
    {
        var tags = Tagger.ReadTags(file);
        tags.Genre = settings.Genre is not null ? GetMultiValue(settings.Genre) : tags.Genre;
        tags.Artist = settings.Artist is not null ? GetMultiValue(settings.Artist) : tags.Artist;
        tags.AlbumArtist = settings.AlbumArtist is not null
            ? GetMultiValue(settings.AlbumArtist)
            : tags.AlbumArtist;
        tags.Title = settings.Title ?? tags.Title;
        tags.Album = settings.Album ?? tags.Album;
        tags.Year = settings.Year ?? tags.Year;
        tags.Label = settings.Label ?? tags.Label;
        tags.CatalogNumber = settings.CatalogNumber ?? tags.CatalogNumber;
        tags.Track = settings.Track ?? tags.Track;
        ctx.Status("Writing tags...");
        Tagger.WriteTags(file, tags);
        Console.PrintTagData(Tagger.ReadTags(file));
        await Task.CompletedTask;
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
