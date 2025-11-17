namespace TagSelecta.Tagging;

public class TagData
{
    [TagDataField("Album")]
    public string Album { get; set; } = "";

    [TagDataField("Album Artist")]
    public List<string> AlbumArtists { get; set; } = [];

    [TagDataField("Artist")]
    public List<string> Artists { get; set; } = [];

    [TagDataField("BPM")]
    public string Bpm { get; set; } = "";

    [TagDataField("Catalog Number")]
    public string CatalogNumber { get; set; } = "";

    [TagDataField("Comment")]
    public string Comment { get; set; } = "";

    [TagDataField("Composers")]
    public List<string> Composers { get; set; } = [];

    [TagDataField("Conductor")]
    public string Conductor { get; set; } = "";

    [TagDataField("Date")]
    public string Date { get; set; } = "";

    [TagDataField("Disc")]
    public string Disc { get; set; } = "";

    [TagDataField("Disc Total")]
    public string DiscTotal { get; set; } = "";

    [TagDataField("Discogs Release Id")]
    public string DiscogsReleaseId { get; set; } = "";

    [TagDataField("Genre")]
    public List<string> Genres { get; set; } = [];

    [TagDataField("ISRC")]
    public string Isrc { get; set; } = "";

    [TagDataField("Label")]
    public string Label { get; set; } = "";

    [TagDataField("Title")]
    public string Title { get; set; } = "";

    [TagDataField("Track")]
    public string Track { get; set; } = "";

    [TagDataField("Track Total")]
    public string TrackTotal { get; set; } = "";

    [TagDataField("Pictures")]
    public List<TagLib.Picture> Pictures { get; set; } = [];

    public List<CustomField> Custom { get; set; } = [];

    public TagData Clone()
    {
        return TagDataCloner.Clone(this);
    }
}
