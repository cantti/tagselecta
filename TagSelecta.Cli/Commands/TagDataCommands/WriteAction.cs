using Spectre.Console;
using Spectre.Console.Cli;

namespace TagSelecta.Cli.Commands.TagDataCommands;

public class WriteSettings : BaseSettings
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

    [CommandOption("--composers")]
    public string[]? Composers { get; set; }

    [CommandOption("--custom")]
    public string[]? Custom { get; set; }
}

public class WriteAction : TagDataAction<WriteSettings>
{
    protected override bool BeforeProcessTagData(TagDataActionContext<WriteSettings> context)
    {
        // convert arrays with empty first element to empty arrays
        foreach (var prop in typeof(WriteSettings).GetProperties())
        {
            if (prop.Name == nameof(WriteSettings.Path))
                continue;
            var val = prop.GetValue(context.Settings);
            if (val is null)
                continue;
            if (val is string[] valArray)
            {
                if (valArray.First() == "")
                {
                    prop.SetValue(context.Settings, Array.Empty<string>());
                }
            }
        }
        return true;
    }

    protected override void ProcessTagData(TagDataActionContext<WriteSettings> context)
    {
        var settings = context.Settings;
        var tagData = context.TagData;

        if (settings.Album is not null)
        {
            tagData.Album = settings.Album;
        }

        if (settings.AlbumArtist is not null)
        {
            tagData.AlbumArtists = [.. settings.AlbumArtist];
        }

        if (settings.Artist is not null)
        {
            tagData.Artists = [.. settings.Artist];
        }

        if (settings.Comment is not null)
        {
            tagData.Comment = settings.Comment;
        }

        if (settings.Composers is not null)
        {
            tagData.Composers = [.. settings.Composers];
        }

        if (settings.Disc is not null)
        {
            tagData.Disc = settings.Disc.Value;
        }

        if (settings.DiscTotal is not null)
        {
            tagData.DiscTotal = settings.DiscTotal.Value;
        }

        if (settings.Genre is not null)
        {
            tagData.Genres = [.. settings.Genre];
        }

        if (settings.Title is not null)
        {
            tagData.Title = settings.Title;
        }

        if (settings.Track is not null)
        {
            tagData.Track = settings.Track.Value;
        }

        if (settings.TrackTotal is not null)
        {
            tagData.TrackTotal = settings.TrackTotal.Value;
        }

        if (settings.Year is not null)
        {
            tagData.Year = settings.Year.Value;
        }

        Console.WriteLine("Before");
        Console.WriteLine(tagData.Custom.Count);
        if (settings.Custom is not null)
        {
            foreach (var entry in settings.Custom)
            {
                var parts = entry.Split('=', 2);
                var key = parts[0].Trim().ToLower();
                var value = parts.Length > 1 ? parts[1].Trim() : "";

                tagData.Custom[key] = value;
            }
        }
        Console.WriteLine("After");
        Console.WriteLine(tagData.Custom.Count);
    }
}
