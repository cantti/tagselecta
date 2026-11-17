using TagSelecta.Commands.Tui;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.App.Config;

public class ConfigModel
{
    public GeneralSection General { get; set; } = new();
    public Dictionary<string, string> Macros { get; set; } = new();
    public DiscogsSection Discogs { get; set; } = new();
    public MusicBrainzSection MusicBrainz { get; set; } = new();
    public ColorScheme ColorScheme { get; set; } = new();
    public Dictionary<string, string> FieldIcons { get; set; } =
        new()
        {
            ["path"] = "\uf07b",
            [FieldName.Artist] = "\uf007",
            [FieldName.AlbumArtist] = "\uf0c0",
            [FieldName.Album] = "\U000f0025",
            [FieldName.Bpm] = "\U000f07da",
            [FieldName.Comment] = "\uf075",
            [FieldName.Composer] = "\uf040",
            [FieldName.Conductor] = "\uf007",
            [FieldName.Copyright] = "\uf1f9",
            [FieldName.DiscNumber] = "\uede9",
            [FieldName.DiscTotal] = "\uf1c0",
            [FieldName.Isrc] = "\uf02a",
            [FieldName.Publisher] = "\uf1ad",
            [FieldName.Title] = "\uf001",
            [FieldName.TrackNumber] = "\uf292",
            [FieldName.TrackTotal] = "\uf0ca",
            [FieldName.Date] = "\uf073",
            [FieldName.Genre] = "\uf02b",
            ["picture"] = "\uf03e",
        };
}
