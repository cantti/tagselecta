using TagSelecta.Shared;

namespace TagSelecta.Tagging;

public class TagDataForTemplate(TagData tagData, string path)
{
    public string Path => path;

    public string FileName => System.IO.Path.GetFileNameWithoutExtension(Path);

    public string Album => tagData.Album;

    public string AlbumArtist => tagData.AlbumArtist.ToJoined();

    public List<string> AlbumArtists => tagData.AlbumArtist;

    public string Artist => tagData.Artist.ToJoined();

    public List<string> Artists => tagData.Artist;

    public string Bpm => tagData.Bpm;

    public string CatalogNumber => tagData.CatalogNumber;

    public string Comment => tagData.Comment;

    public string Composer => tagData.Composer.ToJoined();

    public List<string> Composers => tagData.Composer;

    public string Conductor => tagData.Conductor;

    public string Copyright => tagData.Copyright;

    public string Date => tagData.Date;

    public string Disc => tagData.Disc;

    public string DiscTotal => tagData.DiscTotal;

    public string DiscogsReleaseId => tagData.DiscogsReleaseId;

    public string Genre => tagData.Genre.ToJoined();

    public List<string> Genres => tagData.Genre;

    public string Isrc => tagData.Isrc;

    public string Label => tagData.Label;

    public string Publisher => tagData.Publisher;

    public string Title => tagData.Title;

    public string Track => tagData.Track;

    public string TrackTotal => tagData.TrackTotal;

    public string Year => DateTime.TryParse(tagData.Date, out var d) ? d.Year.ToString() : "";

    public Dictionary<string, string> Custom =>
        tagData.Custom.ToDictionary(x => x.Key, x => x.Text);
}
