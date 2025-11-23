namespace TagSelecta.Tagging;

public class TagData
{
    [TagDataField("Album")]
    public string Album { get; set; } = "";

    [TagDataField("Album Artist")]
    public List<string> AlbumArtist { get; set; } = [];

    [TagDataField("Artist")]
    public List<string> Artist { get; set; } = [];

    [TagDataField("BPM")]
    public string Bpm { get; set; } = "";

    [TagDataField("Catalog Number")]
    public string CatalogNumber { get; set; } = "";

    [TagDataField("Comment")]
    public string Comment { get; set; } = "";

    [TagDataField("Composers")]
    public List<string> Composer { get; set; } = [];

    [TagDataField("Conductor")]
    public string Conductor { get; set; } = "";

    [TagDataField("Copyright")]
    public string Copyright { get; set; } = "";

    [TagDataField("Date")]
    public string Date { get; set; } = "";

    [TagDataField("Disc")]
    public string Disc { get; set; } = "";

    [TagDataField("Disc Total")]
    public string DiscTotal { get; set; } = "";

    [TagDataField("Discogs Release Id")]
    public string DiscogsReleaseId { get; set; } = "";

    [TagDataField("Genre")]
    public List<string> Genre { get; set; } = [];

    [TagDataField("ISRC")]
    public string Isrc { get; set; } = "";

    [TagDataField("Label")]
    public string Label { get; set; } = "";

    [TagDataField("Publisher")]
    public string Publisher { get; set; } = "";

    [TagDataField("Title")]
    public string Title { get; set; } = "";

    [TagDataField("Track")]
    public string Track { get; set; } = "";

    [TagDataField("Track Total")]
    public string TrackTotal { get; set; } = "";

    public List<TagLib.Picture> Picture { get; set; } = [];

    public List<CustomField> Custom { get; set; } = [];

    public TagData Clone()
    {
        return TagDataCloner.Clone(this);
    }
}
