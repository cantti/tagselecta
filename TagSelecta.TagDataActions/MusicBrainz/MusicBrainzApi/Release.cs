namespace TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

public class Release
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? Disambiguation { get; set; }
    public List<ArtistCredit>? ArtistCredit { get; set; }
    public string? Date { get; set; }
    public string? Country { get; set; }
    public List<ReleaseEvent>? ReleaseEvents { get; set; }
    public List<LabelInfo>? LabelInfo { get; set; }
    public string? Barcode { get; set; }
    public string? PackagingId { get; set; }
    public string? Packaging { get; set; }
    public string? StatusId { get; set; }
    public string? Status { get; set; }
    public string? Quality { get; set; }
    public TextRepresentation? TextRepresentation { get; set; }
    public string? Asin { get; set; }
    public List<Media>? Media { get; set; }
    public CoverArtArchive? CoverArtArchive { get; set; }
}
