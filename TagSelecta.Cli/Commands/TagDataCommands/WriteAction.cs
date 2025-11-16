using System.ComponentModel;
using Spectre.Console.Cli;
using TagSelecta.Shared;
using TagSelecta.Tagging;

namespace TagSelecta.Cli.Commands.TagDataCommands;

public class WriteSettings : BaseSettings
{
    [CommandOption("--artist|-a")]
    [Description("One or more artists. Multiple values can be provided using a ';' separator")]
    public string? Artist { get; set; }

    [CommandOption("--albumartist|-A")]
    [Description(
        "One or more album artists. Multiple values can be provided using a ';' separator."
    )]
    public string? AlbumArtist { get; set; }

    [CommandOption("--title|-t")]
    [Description("Track title.")]
    public string? Title { get; set; }

    [CommandOption("--album|-l")]
    [Description("Album name.")]
    public string? Album { get; set; }

    [CommandOption("--year|-y")]
    [Description("Release year.")]
    public int? Year { get; set; }

    [CommandOption("--genre|-g")]
    [Description("One or more genres. Multiple values can be provided using a ';' separator")]
    public string? Genre { get; set; }

    [CommandOption("--track|-n")]
    [Description("Track number.")]
    public int? Track { get; set; }

    [CommandOption("--tracktotal|-N")]
    [Description("Total number of tracks.")]
    public int? TrackTotal { get; set; }

    [CommandOption("--comment|-C")]
    [Description("Comment or notes.")]
    public string? Comment { get; set; }

    [CommandOption("--disc|-d")]
    [Description("Disc number.")]
    public int? Disc { get; set; }

    [CommandOption("--disctotal|-D")]
    [Description("Total number of discs.")]
    public int? DiscTotal { get; set; }

    [CommandOption("--composers")]
    [Description("One or more composers. Multiple values can be provided using a ';' separator")]
    public string? Composer { get; set; }

    [CommandOption("--label")]
    [Description("Record label.")]
    public string? Label { get; set; }

    [CommandOption("--catalognumber")]
    [Description("Catalog number.")]
    public string? CatallogNumber { get; set; }

    [CommandOption("--custom|-c")]
    [Description(
        "Custom tags in key=value format. Multiple entries can be provided using a ';' separator (e.g., key1=val1;key2=val2)."
    )]
    public string? Custom { get; set; }

    [CommandOption("--clearcustom")]
    [Description("Clear all other custom tags, not specified using --custom or -c")]
    public bool ClearCustom { get; set; }
}

public class WriteAction : TagDataAction<WriteSettings>
{
    protected override void ProcessTagData(TagDataActionContext<WriteSettings> context)
    {
        var settings = context.Settings;
        var tagData = context.TagData;
        var formatter = new TagDataFormatter(tagData.Clone(), context.CurrentFile);

        if (settings.Album is not null)
        {
            tagData.Album = formatter.Format(settings.Album);
        }

        if (settings.AlbumArtist is not null)
        {
            tagData.AlbumArtists = formatter.Format(settings.AlbumArtist).ToMulti();
        }

        if (settings.Artist is not null)
        {
            tagData.Artists = formatter.Format(settings.Artist).ToMulti();
        }

        if (settings.Comment is not null)
        {
            tagData.Comment = formatter.Format(settings.Comment);
        }

        if (settings.Composer is not null)
        {
            tagData.Composers = formatter.Format(settings.Composer).ToMulti();
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
            tagData.Genres = formatter.Format(settings.Genre).ToMulti();
        }

        if (settings.Title is not null)
        {
            tagData.Title = formatter.Format(settings.Title);
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
            tagData.Label = formatter.Format(settings.Label);
        }

        if (settings.CatallogNumber is not null)
        {
            tagData.CatalogNumber = formatter.Format(settings.CatallogNumber);
        }

        if (settings.ClearCustom)
        {
            tagData.Custom = [];
        }

        if (settings.Custom is not null)
        {
            var custom = settings.Custom.ToMulti();
            foreach (var entry in custom)
            {
                var parts = entry.Split('=', 2);
                var key = parts[0].Trim().ToLower();
                var value = parts.Length > 1 ? parts[1].Trim() : "";

                value = formatter.Format(value);

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
