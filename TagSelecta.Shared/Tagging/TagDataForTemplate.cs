using System.ComponentModel;
using System.Globalization;

namespace TagSelecta.Shared.Tagging;

public class TagDataForTemplate(TagData tagData, string path)
{
    [Description("Full file path.")]
    public string Path { get; } = path;

    [Description("File name without extension.")]
    public string FileName { get; } = System.IO.Path.GetFileNameWithoutExtension(path);

    [Description("File extension")]
    public string Ext { get; } = System.IO.Path.GetExtension(path).TrimStart('.');

    [Description("Album name.")]
    public string Album { get; } = tagData.Album;

    [Description("Album artists as a single string.")]
    public string AlbumArtist { get; } = tagData.AlbumArtist.ToJoined();

    [Description("List of album artists.")]
    public List<string> AlbumArtists { get; } = tagData.AlbumArtist;

    [Description("Artists as a single string.")]
    public string Artist { get; } = tagData.Artist.ToJoined();

    [Description("List of artists.")]
    public List<string> Artists { get; } = tagData.Artist;

    [Description("Beats per minute.")]
    public string Bpm { get; } = tagData.Bpm;

    [Description("Catalog number.")]
    public string CatalogNumber { get; } = tagData.CatalogNumber;

    [Description("User comment.")]
    public string Comment { get; } = tagData.Comment;

    [Description("Composers as a single string.")]
    public string Composer { get; } = tagData.Composer.ToJoined();

    [Description("List of composers.")]
    public List<string> Composers { get; } = tagData.Composer;

    [Description("Conductor name.")]
    public string Conductor { get; } = tagData.Conductor;

    [Description("Copyright text.")]
    public string Copyright { get; } = tagData.Copyright;

    [Description("Original date value.")]
    public string Date { get; } = tagData.Date;

    [Description("Disc number.")]
    public string Disc { get; } = tagData.Disc;

    [Description("Disc number padded with zeros (e.g. 01, 02, 03, etc.)")]
    public string Disc00 { get; } =
        int.TryParse(tagData.Disc, out var disc) ? disc.ToString("D2") : tagData.Disc;

    [Description("Total number of discs.")]
    public string DiscTotal { get; } = tagData.DiscTotal;

    [Description("Genres as a single string.")]
    public string Genre { get; } = tagData.Genre.ToJoined();

    [Description("List of genres.")]
    public List<string> Genres { get; } = tagData.Genre;

    [Description("ISRC code.")]
    public string Isrc { get; } = tagData.Isrc;

    [Description("Record label.")]
    public string Label { get; } = tagData.Label;

    [Description("Publisher.")]
    public string Publisher { get; } = tagData.Publisher;

    [Description("Track title.")]
    public string Title { get; } = tagData.Title;

    [Description("Track number.")]
    public string Track { get; } = tagData.Track;

    [Description("Track number padded with zeros (e.g. 01, 02, 03, etc.)")]
    public string Track00 { get; } =
        int.TryParse(tagData.Track, out var track) ? track.ToString("D2") : tagData.Track;

    [Description("Total number of tracks.")]
    public string TrackTotal { get; } = tagData.TrackTotal;

    [Description("Year extracted from the Date field.")]
    public string Year { get; } =
        DateTime.TryParseExact(
            tagData.Date,
            ["yyyy", "yyyy-MM-dd", "yyyy/MM/dd"],
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var d
        )
            ? d.Year.ToString()
            : "";

    [Description("Custom fields. Usage example: custom.url")]
    public Dictionary<string, string> Custom { get; } =
        tagData.Custom.ToDictionary(x => x.Key, x => x.Text);
}
