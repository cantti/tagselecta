using System.ComponentModel;
using Spectre.Console.Cli;
using TagSelecta.Cli.Commands.TagDataCommands.Common;
using TagSelecta.Shared;
using TagSelecta.Tagging;

namespace TagSelecta.Cli.Commands.TagDataCommands;

public class WriteSettings : BaseSettings
{
    [CommandOption($"--{Fields.Album}|-l")]
    [Description("Album name.")]
    public string? Album { get; set; }

    [CommandOption($"--{Fields.AlbumArtist}|-A")]
    [Description(
        "One or more album artists. Multiple values can be provided using a ';' separator."
    )]
    public string? AlbumArtist { get; set; }

    [CommandOption($"--{Fields.Artist}|-a")]
    [Description("One or more artists. Multiple values can be provided using a ';' separator")]
    public string? Artist { get; set; }

    [CommandOption($"--{Fields.Bpm}")]
    [Description("Beat per minutes")]
    public string? Bpm { get; set; }

    [CommandOption($"--{Fields.CatalogNumber}")]
    [Description("Catalog number.")]
    public string? CatalogNumber { get; set; }

    [CommandOption($"--{Fields.Comment}|-C")]
    [Description("Comment or notes.")]
    public string? Comment { get; set; }

    [CommandOption($"--{Fields.Composer}")]
    [Description("Composer.")]
    public string? Composer { get; set; }

    [CommandOption($"--{Fields.Conductor}")]
    [Description("Conductor.")]
    public string? Conductor { get; set; }

    [CommandOption($"--{Fields.Copyright}")]
    [Description("Copyright.")]
    public string? Copyright { get; set; }

    [CommandOption($"--{Fields.Date}|-y")]
    [Description("Release date.")]
    public string? Date { get; set; }

    [CommandOption($"--{Fields.Disc}|-d")]
    [Description("Disc number.")]
    public string? Disc { get; set; }

    [CommandOption($"--{Fields.DiscTotal}|-D")]
    [Description("Total number of discs.")]
    public string? DiscTotal { get; set; }

    [CommandOption($"--{Fields.DiscogsReleaseId}")]
    [Description("Discogs release id.")]
    public string? DiscogsReleaseId { get; set; }

    [CommandOption($"--{Fields.Genre}|-g")]
    [Description("One or more genres. Multiple values can be provided using a ';' separator")]
    public string? Genre { get; set; }

    [CommandOption($"--{Fields.Isrc}")]
    [Description("International standard recording code")]
    public string? Isrc { get; set; }

    [CommandOption($"--{Fields.Label}")]
    [Description("Record label.")]
    public string? Label { get; set; }

    [CommandOption($"--{Fields.Publisher}")]
    [Description("Publisher.")]
    public string? Publisher { get; set; }

    [CommandOption($"--{Fields.Title}|-t")]
    [Description("Track title.")]
    public string? Title { get; set; }

    [CommandOption($"--{Fields.Track}|-n")]
    [Description("Track number.")]
    public string? Track { get; set; }

    [CommandOption($"--{Fields.TrackTotal}|-N")]
    [Description("Total number of tracks.")]
    public string? TrackTotal { get; set; }

    [CommandOption($"--{Fields.Custom}|-c")]
    [Description(
        "Custom tags in key=value format. Use this option multiple times to include multiple fields (e.g., -c key1=value1 -c key2=value2)."
    )]
    public string[]? Custom { get; set; }

    [CommandOption("--clearcustom")]
    [Description("Clear all other custom tags, not specified using --custom or -c")]
    public bool ClearCustom { get; set; }

    [CommandOption($"--{Fields.Picture}|-p")]
    [Description(
        "Path to a picture file. Use this option multiple times to include multiple images (e.g., -p path1 -p path2)."
    )]
    public string[]? Picture { get; set; }

    [CommandOption($"--{Fields.PictureType}")]
    [Description(
        "Type of each picture provided. Specify multiple times to match the order of the pictures. This option is optional.\nCommon values: FrontCover, BackCover, Artist, Other."
    )]
    public string[]? PictureType { get; set; }

    [CommandOption("--clearpicture")]
    [Description("Clear all other pictures")]
    public bool ClearPicture { get; set; }
}

public class WriteAction : TagDataAction<WriteSettings>
{
    protected override void ProcessTagData(ITagDataActionContext<WriteSettings> context)
    {
        var settings = context.Settings;
        var tagData = context.TagData;
        var formatter = new TagDataFormatter(tagData.Clone(), context.CurrentFile);

        var map = new (Func<WriteSettings, object?> get, Action<string> set)[]
        {
            (s => s.Album, v => tagData.Album = v),
            (s => s.AlbumArtist, v => tagData.AlbumArtist = v.ToMulti()),
            (s => s.Artist, v => tagData.Artist = v.ToMulti()),
            (s => s.Bpm, v => tagData.Bpm = v),
            (s => s.CatalogNumber, v => tagData.CatalogNumber = v),
            (s => s.Comment, v => tagData.Comment = v),
            (s => s.Composer, v => tagData.Composer = v.ToMulti()),
            (s => s.Conductor, v => tagData.Conductor = v),
            (s => s.Copyright, v => tagData.Copyright = v),
            (s => s.Date, v => tagData.Date = v),
            (s => s.Disc, v => tagData.Disc = v),
            (s => s.DiscTotal, v => tagData.DiscTotal = v),
            (s => s.DiscogsReleaseId, v => tagData.DiscogsReleaseId = v),
            (s => s.Genre, v => tagData.Genre = v.ToMulti()),
            (s => s.Isrc, v => tagData.Isrc = v),
            (s => s.Label, v => tagData.Label = v),
            (s => s.Publisher, v => tagData.Publisher = v),
            (s => s.Title, v => tagData.Title = v),
            (s => s.Track, v => tagData.Track = v),
            (s => s.TrackTotal, v => tagData.TrackTotal = v),
            (s => s.TrackTotal, v => tagData.TrackTotal = v),
        };

        foreach (var (get, set) in map)
        {
            var value = get(settings);
            if (value == null)
            {
                continue;
            }
            var str = (string)value;
            str = formatter.Format(str);
            set(str);
        }

        if (settings.ClearCustom)
        {
            tagData.Custom = [];
        }

        if (settings.Custom is not null)
        {
            foreach (var entry in settings.Custom)
            {
                var parts = entry.Split('=', 2);
                var key = parts[0].Trim().ToLower();
                var value = parts.Length > 1 ? parts[1].Trim() : "";

                value = formatter.Format(value);

                var customTagData = tagData.Custom.SingleOrDefault(x => x.Key == key);

                if (customTagData is not null)
                {
                    customTagData.Text = value;
                }
                else
                {
                    tagData.Custom.Add(new CustomField(key, value));
                }
            }
        }

        if (settings.ClearPicture)
        {
            tagData.Picture = [];
        }

        if (settings.Picture is not null)
        {
            for (int i = 0; i < settings.Picture.Length; i++)
            {
                var path = settings.Picture[i];
                // try to find corresponding picture type, or use first
                var typeStr =
                    settings.PictureType?.ElementAtOrDefault(i)
                    ?? settings.PictureType?.FirstOrDefault();
                var picture = new TagLib.Picture(path)
                {
                    Type =
                        !string.IsNullOrEmpty(typeStr)
                        && Enum.TryParse<TagLib.PictureType>(typeStr, true, out var type)
                            ? type
                            : TagLib.PictureType.FrontCover,
                };
                tagData.Picture.Add(picture);
            }
        }
    }
}
