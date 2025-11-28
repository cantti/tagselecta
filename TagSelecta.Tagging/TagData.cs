namespace TagSelecta.Tagging;

public class TagData
{
    [BuiltinField("Album")]
    public string Album { get; set; } = "";

    [BuiltinField("Album Artist")]
    public List<string> AlbumArtist { get; set; } = [];

    [BuiltinField("Artist")]
    public List<string> Artist { get; set; } = [];

    [BuiltinField("BPM")]
    public string Bpm { get; set; } = "";

    [BuiltinField("Catalog Number")]
    public string CatalogNumber { get; set; } = "";

    [BuiltinField("Comment")]
    public string Comment { get; set; } = "";

    [BuiltinField("Composers")]
    public List<string> Composer { get; set; } = [];

    [BuiltinField("Conductor")]
    public string Conductor { get; set; } = "";

    [BuiltinField("Copyright")]
    public string Copyright { get; set; } = "";

    [BuiltinField("Date")]
    public string Date { get; set; } = "";

    [BuiltinField("Disc")]
    public string Disc { get; set; } = "";

    [BuiltinField("Disc Total")]
    public string DiscTotal { get; set; } = "";

    [BuiltinField("Discogs Release Id")]
    public string DiscogsReleaseId { get; set; } = "";

    [BuiltinField("Genre")]
    public List<string> Genre { get; set; } = [];

    [BuiltinField("ISRC")]
    public string Isrc { get; set; } = "";

    [BuiltinField("Label")]
    public string Label { get; set; } = "";

    [BuiltinField("Publisher")]
    public string Publisher { get; set; } = "";

    [BuiltinField("Title")]
    public string Title { get; set; } = "";

    [BuiltinField("Track")]
    public string Track { get; set; } = "";

    [BuiltinField("Track Total")]
    public string TrackTotal { get; set; } = "";

    public List<TagLib.Picture> Picture { get; set; } = [];

    public List<CustomField> Custom { get; set; } = [];

    public TagData Clone()
    {
        return TagDataCloner.Clone(this);
    }
}
