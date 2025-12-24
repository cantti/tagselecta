namespace TagSelecta.Tagging;

public class TagData
{
    public string Album { get; set; } = "";

    public List<string> AlbumArtist { get; set; } = [];

    public List<string> Artist { get; set; } = [];

    public string Bpm { get; set; } = "";

    public string CatalogNumber { get; set; } = "";

    public string Comment { get; set; } = "";

    public List<string> Composer { get; set; } = [];

    public string Conductor { get; set; } = "";

    public string Copyright { get; set; } = "";

    public string Date { get; set; } = "";

    public string Disc { get; set; } = "";

    public string DiscTotal { get; set; } = "";

    public string DiscogsReleaseId { get; set; } = "";

    public List<string> Genre { get; set; } = [];

    public string Isrc { get; set; } = "";

    public string Label { get; set; } = "";

    public string Publisher { get; set; } = "";

    public string Title { get; set; } = "";

    public string Track { get; set; } = "";

    public string TrackTotal { get; set; } = "";

    public List<TagLib.Picture> Picture { get; set; } = [];

    public List<CustomField> Custom { get; set; } = [];

    public TagData Clone()
    {
        return TagDataCloner.Clone(this);
    }
}
