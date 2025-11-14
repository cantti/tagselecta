using TagSelecta.Shared;

namespace TagSelecta.Tagging;

public class TagData
{
    public string Path { get; set; } = "";

    public string FileName => System.IO.Path.GetFileNameWithoutExtension(Path);

    [Printable("Album Artist")]
    public List<string> AlbumArtists { get; set; } = [];

    public string AlbumArtist => AlbumArtists.Joined();

    [Printable("Artist")]
    public List<string> Artists { get; set; } = [];

    public string Artist => Artists.Joined();

    [Printable]
    public string Album { get; set; } = "";

    [Printable]
    public int Year { get; set; }

    [Printable]
    public string Title { get; set; } = "";

    [Printable]
    public int Track { get; set; }

    [Printable("Track Total")]
    public int TrackTotal { get; set; }

    [Printable]
    public int Disc { get; set; }

    [Printable("Disc Total")]
    public int DiscTotal { get; set; }

    [Printable("Genre")]
    public List<string> Genres { get; set; } = [];

    public string Genre => Genres.Joined();

    [Printable]
    public string Comment { get; set; } = "";

    [Printable]
    public List<string> Composers { get; set; } = [];

    public string Composer => Composers.Joined();

    public List<CustomField> Custom { get; set; } = [];

    [Printable]
    public List<TagLib.Picture> Pictures { get; set; } = [];

    [Printable]
    public string Label { get; set; } = "";

    [Printable("Catalog Number")]
    public string CatalogNumber { get; set; } = "";

    [Printable("Discogs Release Id")]
    public string DiscogsReleaseId { get; set; } = "";

    public TagData Clone()
    {
        return TagDataCloner.Clone(this);
    }
}
