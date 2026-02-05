using System.ComponentModel;
using System.Globalization;

namespace TagSelecta.Shared.Tagging;

public class TagDataForTemplate
{
    [Description("Full file path.")]
    public string? Path { get; set; }

    [Description("File name without extension.")]
    public string? FileName { get; set; }

    [Description("File extension")]
    public string? Ext { get; set; }

    [Description("Album name.")]
    public string? Album { get; set; }

    [Description("Album artists as a single string.")]
    public string? AlbumArtist { get; set; }

    [Description("List of album artists.")]
    public List<string> AlbumArtists { get; set; } = [];

    [Description("Artists as a single string.")]
    public string? Artist { get; set; }

    [Description("List of artists.")]
    public List<string> Artists { get; set; } = [];

    [Description("Beats per minute.")]
    public string? Bpm { get; set; }

    [Description("Catalog number.")]
    public string? CatalogNumber { get; set; }

    [Description("User comment.")]
    public string? Comment { get; set; }

    [Description("Composers as a single string.")]
    public string? Composer { get; set; }

    [Description("List of composers.")]
    public List<string> Composers { get; set; } = [];

    [Description("Conductor name.")]
    public string? Conductor { get; set; }

    [Description("Copyright text.")]
    public string? Copyright { get; set; }

    [Description("Original date value.")]
    public string? Date { get; set; }

    [Description("Disc number.")]
    public string? Disc { get; set; }

    [Description("Total number of discs.")]
    public string? DiscTotal { get; set; }

    [Description("Genres as a single string.")]
    public string? Genre { get; set; }

    [Description("List of genres.")]
    public List<string> Genres { get; set; } = [];

    [Description("ISRC code.")]
    public string? Isrc { get; set; }

    [Description("Record label.")]
    public string? Label { get; set; }

    [Description("Publisher.")]
    public string? Publisher { get; set; }

    [Description("Track title.")]
    public string? Title { get; set; }

    [Description("Track number.")]
    public string? Track { get; set; }

    [Description("Total number of tracks.")]
    public string? TrackTotal { get; set; }

    [Description("Year extracted from the Date field.")]
    public string? Year { get; set; }

    [Description("Extra fields. Usage example: extra.url")]
    public Dictionary<string, string> Extra { get; set; } = [];
}
