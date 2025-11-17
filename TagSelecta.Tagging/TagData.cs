namespace TagSelecta.Tagging;

public class TagData
{
    // 1. Album
    [TagDataField("Album")]
    public string Album { get; set; } = "";

    // 2. AlbumArtists
    [TagDataField("Album Artist")]
    public List<string> AlbumArtists { get; set; } = [];

    // 3. Artists
    [TagDataField("Artist")]
    public List<string> Artists { get; set; } = [];

    // 4. Bpm
    [TagDataField("BPM")]
    public string Bpm { get; set; } = "";

    // 5. CatalogNumber
    [TagDataField("Catalog Number")]
    public string CatalogNumber { get; set; } = "";

    // 6. Comment
    [TagDataField("Comment")]
    public string Comment { get; set; } = "";

    // 7. Composers
    [TagDataField("Composers")]
    public List<string> Composers { get; set; } = [];

    // 8. Conductor
    [TagDataField("Conductor")]
    public string Conductor { get; set; } = "";

    // 9. Date
    [TagDataField("Date")]
    public string Date { get; set; } = "";

    // 10. Disc
    [TagDataField("Disc")]
    public string Disc { get; set; } = "";

    // 11. DiscTotal
    [TagDataField("Disc Total")]
    public string DiscTotal { get; set; } = "";

    // 12. DiscogsReleaseId
    [TagDataField("Discogs Release Id")]
    public string DiscogsReleaseId { get; set; } = "";

    // 13. Genres
    [TagDataField("Genre")]
    public List<string> Genres { get; set; } = [];

    // 14. Isrc
    [TagDataField("ISRC")]
    public string Isrc { get; set; } = "";

    // 15. Label
    [TagDataField("Label")]
    public string Label { get; set; } = "";

    // 16. Title
    [TagDataField("Title")]
    public string Title { get; set; } = "";

    // 17. Track
    [TagDataField("Track")]
    public string Track { get; set; } = "";

    // 18. TrackTotal
    [TagDataField("Track Total")]
    public string TrackTotal { get; set; } = "";

    // Pictures (unnumbered)
    [TagDataField("Pictures")]
    public List<TagLib.Picture> Pictures { get; set; } = [];

    // Custom (unnumbered, very last)
    public List<CustomField> Custom { get; set; } = [];

    public TagData Clone()
    {
        return TagDataCloner.Clone(this);
    }
}
