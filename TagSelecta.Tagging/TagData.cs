namespace TagSelecta.Tagging;

public class TagData
{
    [TagDataField("Album Artist")]
    public List<string> AlbumArtists { get; set; } = [];

    [TagDataField("Artist")]
    public List<string> Artists { get; set; } = [];

    [TagDataField("Album")]
    public string Album { get; set; } = "";

    [TagDataField("Year")]
    public int Year { get; set; }

    [TagDataField("Title")]
    public string Title { get; set; } = "";

    [TagDataField("Track")]
    public int Track { get; set; }

    [TagDataField("Track Total")]
    public int TrackTotal { get; set; }

    [TagDataField("Disc")]
    public int Disc { get; set; }

    [TagDataField("Disc Total")]
    public int DiscTotal { get; set; }

    [TagDataField("Genre")]
    public List<string> Genres { get; set; } = [];

    [TagDataField("Comment")]
    public string Comment { get; set; } = "";

    [TagDataField("Composers")]
    public List<string> Composers { get; set; } = [];

    [TagDataField("Pictures")]
    public List<TagLib.Picture> Pictures { get; set; } = [];

    [TagDataField("Label")]
    public string Label { get; set; } = "";

    [TagDataField("Catalog Number")]
    public string CatalogNumber { get; set; } = "";

    [TagDataField("Discogs Release Id")]
    public string DiscogsReleaseId { get; set; } = "";

    public List<CustomField> Custom { get; set; } = [];

    public TagData Clone()
    {
        return TagDataCloner.Clone(this);
    }
}
