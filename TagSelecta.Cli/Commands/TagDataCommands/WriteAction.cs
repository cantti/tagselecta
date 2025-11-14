using Spectre.Console.Cli;
using TagSelecta.Tagging;

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
    public int? Year { get; set; }

    [CommandOption("--track|-n")]
    public int? Track { get; set; }

    [CommandOption("--tracktotal|-N")]
    public int? TrackTotal { get; set; }

    [CommandOption("--comment|-c")]
    public string? Comment { get; set; }

    [CommandOption("--disc|-d")]
    public int? Disc { get; set; }

    [CommandOption("--disctotal|-D")]
    public int? DiscTotal { get; set; }

    [CommandOption("--composers")]
    public string[]? Composer { get; set; }

    [CommandOption("--label")]
    public string? Label { get; set; }

    [CommandOption("--catalognumber")]
    public string? CatallogNumber { get; set; }

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
        var originalTags = tagData.Clone();

        if (settings.Album is not null)
        {
            tagData.Album = TagDataFormatter.Format(settings.Album, originalTags);
        }

        if (settings.AlbumArtist is not null)
        {
            tagData.AlbumArtists =
            [
                .. settings.AlbumArtist.Select(x => TagDataFormatter.Format(x, originalTags)),
            ];
        }

        if (settings.Artist is not null)
        {
            tagData.Artists =
            [
                .. settings.Artist.Select(x => TagDataFormatter.Format(x, originalTags)),
            ];
        }

        if (settings.Comment is not null)
        {
            tagData.Comment = TagDataFormatter.Format(settings.Comment, originalTags);
        }

        if (settings.Composer is not null)
        {
            tagData.Composers = settings.Composer.ToList();
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
            tagData.Genres =
            [
                .. settings.Genre.Select(x => TagDataFormatter.Format(x, originalTags)),
            ];
        }

        if (settings.Title is not null)
        {
            tagData.Title = TagDataFormatter.Format(settings.Title, originalTags);
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

        if (settings.Label is not null)
        {
            tagData.Label = TagDataFormatter.Format(settings.Label, originalTags);
        }

        if (settings.CatallogNumber is not null)
        {
            tagData.CatalogNumber = TagDataFormatter.Format(settings.CatallogNumber, originalTags);
        }

        if (settings.Custom is not null)
        {
            foreach (var entry in settings.Custom)
            {
                var parts = entry.Split('=', 2);
                var key = parts[0].Trim().ToLower();
                var value = parts.Length > 1 ? parts[1].Trim() : "";

                var customTagData = tagData.Custom.SingleOrDefault(x => x.Key == key);

                if (customTagData is not null)
                {
                    customTagData.Value = value;
                }
                else
                {
                    tagData.Custom.Add(new CustomField(key, value));
                }
            }
        }
    }
}
