using System.ComponentModel;
using Spectre.Console.Cli;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.TagDataActions.Edit;

public class EditSettings : TagDataActionSettings
{
    [CommandOption($"--{FieldName.Album}")]
    [Description("Album name.")]
    public string? Album { get; set; }

    [CommandOption($"--{FieldName.AlbumArtist}")]
    [Description(
        "One or more album artists. Multiple values can be provided using a ';' separator."
    )]
    public string? AlbumArtist { get; set; }

    [CommandOption($"--{FieldName.Artist}")]
    [Description("One or more artists. Multiple values can be provided using a ';' separator")]
    public string? Artist { get; set; }

    [CommandOption($"--{FieldName.Bpm}")]
    [Description("Beat per minutes")]
    public string? Bpm { get; set; }

    [CommandOption($"--{FieldName.Comment}")]
    [Description("Comment or notes.")]
    public string? Comment { get; set; }

    [CommandOption($"--{FieldName.Composer}")]
    [Description("Composer.")]
    public string? Composer { get; set; }

    [CommandOption($"--{FieldName.Conductor}")]
    [Description("Conductor.")]
    public string? Conductor { get; set; }

    [CommandOption($"--{FieldName.Copyright}")]
    [Description("Copyright.")]
    public string? Copyright { get; set; }

    [CommandOption($"--{FieldName.Date}")]
    [Description("Release date.")]
    public string? Date { get; set; }

    [CommandOption($"--{FieldName.DiscNumber}")]
    [Description("Disc number.")]
    public string? DiscNumber { get; set; }

    [CommandOption($"--{FieldName.DiscTotal}")]
    [Description("Total number of discs.")]
    public string? DiscTotal { get; set; }

    [CommandOption($"--{FieldName.Genre}")]
    [Description("One or more genres. Multiple values can be provided using a ';' separator")]
    public string? Genre { get; set; }

    [CommandOption($"--{FieldName.Isrc}")]
    [Description("International standard recording code")]
    public string? Isrc { get; set; }

    [CommandOption($"--{FieldName.Publisher}")]
    [Description("Publisher.")]
    public string? Publisher { get; set; }

    [CommandOption($"--{FieldName.Title}")]
    [Description("Track title.")]
    public string? Title { get; set; }

    [CommandOption($"--{FieldName.TrackNumber}")]
    [Description("Track number.")]
    public string? TrackNumber { get; set; }

    [CommandOption($"--{FieldName.TrackTotal}")]
    [Description("Total number of tracks.")]
    public string? TrackTotal { get; set; }

    [CommandOption("--key|-k")]
    [Description("Extra field key key. Must be used together with --value.")]
    public string[] Key { get; set; } = [];

    [CommandOption("--value|-v")]
    public string[] Value { get; set; } = [];

    [CommandOption("--clear")]
    [Description("Clear all fields.")]
    public bool Clear { get; set; }

    [CommandOption("--picture")]
    [Description(
        "Path or url to a picture. Use this option multiple times to include multiple images (e.g., -p path1 -p path2)."
    )]
    public string[]? Picture { get; set; }

    [CommandOption("--picturetype|-P")]
    [Description(
        "Type of each picture provided. Specify multiple times to match the order of the pictures. This option is optional.\nCommon values: FrontCover, BackCover, Artist, Other."
    )]
    public string[]? PictureType { get; set; }

    [CommandOption("--clearpicture")]
    [Description("Clear all other pictures")]
    public bool ClearPicture { get; set; }
}
