using System.ComponentModel;
using TagSelecta.Shared;

namespace TagSelecta.Tagging;

public class TagDataForTemplate(TagData tagData, string path)
{
    [Description("Full file path.")]
    public string Path => path;

    [Description("File name without extension.")]
    public string FileName => System.IO.Path.GetFileNameWithoutExtension(Path);

    [Description("Album name.")]
    public string Album => tagData.Album;

    [Description("Album artists as a single string.")]
    public string AlbumArtist => tagData.AlbumArtist.ToJoined();

    [Description("List of album artists.")]
    public List<string> AlbumArtists => tagData.AlbumArtist;

    [Description("Artists as a single string.")]
    public string Artist => tagData.Artist.ToJoined();

    [Description("List of artists.")]
    public List<string> Artists => tagData.Artist;

    [Description("Beats per minute.")]
    public string Bpm => tagData.Bpm;

    [Description("Catalog number.")]
    public string CatalogNumber => tagData.CatalogNumber;

    [Description("User comment.")]
    public string Comment => tagData.Comment;

    [Description("Composers as a single string.")]
    public string Composer => tagData.Composer.ToJoined();

    [Description("List of composers.")]
    public List<string> Composers => tagData.Composer;

    [Description("Conductor name.")]
    public string Conductor => tagData.Conductor;

    [Description("Copyright text.")]
    public string Copyright => tagData.Copyright;

    [Description("Original date value.")]
    public string Date => tagData.Date;

    [Description("Disc number.")]
    public string Disc => tagData.Disc;

    [Description("Total number of discs.")]
    public string DiscTotal => tagData.DiscTotal;

    [Description("Discogs release ID.")]
    public string DiscogsReleaseId => tagData.DiscogsReleaseId;

    [Description("Genres as a single string.")]
    public string Genre => tagData.Genre.ToJoined();

    [Description("List of genres.")]
    public List<string> Genres => tagData.Genre;

    [Description("ISRC code.")]
    public string Isrc => tagData.Isrc;

    [Description("Record label.")]
    public string Label => tagData.Label;

    [Description("Publisher.")]
    public string Publisher => tagData.Publisher;

    [Description("Track title.")]
    public string Title => tagData.Title;

    [Description("Track number.")]
    public string Track => tagData.Track;

    [Description("Total number of tracks.")]
    public string TrackTotal => tagData.TrackTotal;

    [Description("Year extracted from the Date field.")]
    public string Year => DateTime.TryParse(tagData.Date, out var d) ? d.Year.ToString() : "";

    [Description("Custom fields. Usage example: custom.url")]
    public Dictionary<string, string> Custom =>
        tagData.Custom.ToDictionary(x => x.Key, x => x.Text);
}
