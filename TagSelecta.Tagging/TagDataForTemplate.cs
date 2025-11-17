using TagSelecta.Shared;

namespace TagSelecta.Tagging;

public class TagDataForTemplate(TagData tagData, string path)
{
    public string Path => path;

    public string FileName => System.IO.Path.GetFileNameWithoutExtension(Path);

    public List<string> AlbumArtists => tagData.AlbumArtists;

    public string AlbumArtist => tagData.AlbumArtists.ToJoined();

    public List<string> Artists => tagData.Artists;

    public string Artist => tagData.Artists.ToJoined();

    public string Album => tagData.Album;

    public string Date => tagData.Date;

    public string Title => tagData.Title;

    public string Track => tagData.Track;

    public string TrackTotal => tagData.TrackTotal;

    public string Disc => tagData.Disc;

    public string DiscTotal => tagData.DiscTotal;

    public List<string> Genres => tagData.Genres;

    public string Genre => tagData.Genres.ToJoined();

    public string Comment => tagData.Comment;

    public List<string> Composers => tagData.Composers;

    public string Composer => tagData.Composers.ToJoined();

    public Dictionary<string, string> Custom =>
        tagData.Custom.ToDictionary(x => x.Key, x => x.Text);

    public List<TagLib.Picture> Pictures => tagData.Pictures;

    public string Label => tagData.Label;

    public string CatalogNumber => tagData.CatalogNumber;

    public string DiscogsReleaseId => tagData.DiscogsReleaseId;
}
