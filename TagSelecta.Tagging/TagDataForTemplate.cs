using TagSelecta.Shared;

namespace TagSelecta.Tagging;

public class TagDataForTemplate(TagData tagData, string path)
{
    public string Path => path;

    public string FileName => System.IO.Path.GetFileNameWithoutExtension(Path);

    public List<string> AlbumArtists => tagData.AlbumArtists;

    public string AlbumArtist => tagData.AlbumArtists.Joined();

    public List<string> Artists => tagData.Artists;

    public string Artist => tagData.Artists.Joined();

    public string Album => tagData.Album;

    public int Year => tagData.Year;

    public string Title => tagData.Title;

    public int Track => tagData.Track;

    public int TrackTotal => tagData.TrackTotal;

    public int Disc => tagData.Disc;

    public int DiscTotal => tagData.DiscTotal;

    public List<string> Genres => tagData.Genres;

    public string Genre => tagData.Genres.Joined();

    public string Comment => tagData.Comment;

    public List<string> Composers => tagData.Composers;

    public string Composer => tagData.Composers.Joined();

    public Dictionary<string, string> Custom =>
        tagData.Custom.ToDictionary(x => x.Key, x => x.Value);

    public List<TagLib.Picture> Pictures => tagData.Pictures;

    public string Label => tagData.Label;

    public string CatalogNumber => tagData.CatalogNumber;

    public string DiscogsReleaseId => tagData.DiscogsReleaseId;
}
