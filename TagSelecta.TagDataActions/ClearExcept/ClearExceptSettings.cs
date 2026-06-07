using Spectre.Console.Cli;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.TagDataActions.ClearExcept;

public class ClearExceptSettings : TagDataActionSettings, ISettingsWithKey
{
    [CommandOption($"--{FieldName.Album}")]
    public bool Album { get; set; }

    [CommandOption($"--{FieldName.AlbumArtist}")]
    public bool AlbumArtist { get; set; }

    [CommandOption($"--{FieldName.Artist}")]
    public bool Artist { get; set; }

    [CommandOption($"--{FieldName.Bpm}")]
    public bool Bpm { get; set; }

    [CommandOption($"--{FieldName.Comment}")]
    public bool Comment { get; set; }

    [CommandOption($"--{FieldName.Composer}")]
    public bool Composer { get; set; }

    [CommandOption($"--{FieldName.Conductor}")]
    public bool Conductor { get; set; }

    [CommandOption($"--{FieldName.Copyright}")]
    public bool Copyright { get; set; }

    [CommandOption($"--{FieldName.Date}")]
    public bool Date { get; set; }

    [CommandOption($"--{FieldName.DiscNumber}")]
    public bool DiscNumber { get; set; }

    [CommandOption($"--{FieldName.DiscTotal}")]
    public bool DiscTotal { get; set; }

    [CommandOption($"--{FieldName.Genre}")]
    public bool Genre { get; set; }

    [CommandOption($"--{FieldName.Isrc}")]
    public bool Isrc { get; set; }

    [CommandOption($"--{FieldName.Publisher}")]
    public bool Publisher { get; set; }

    [CommandOption($"--{FieldName.Title}")]
    public bool Title { get; set; }

    [CommandOption($"--{FieldName.TrackNumber}")]
    public bool TrackNumber { get; set; }

    [CommandOption($"--{FieldName.TrackTotal}")]
    public bool TrackTotal { get; set; }

    [CommandOption("--key|-k")]
    public IEnumerable<string> Key { get; set; } = [];

    [CommandOption("--picture")]
    public bool Picture { get; set; }
}
