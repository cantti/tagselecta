namespace TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

public class Release
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? Disambiguation { get; set; }
    public ArtistCredit[]? ArtistCredit { get; set; }
    public string? Date { get; set; }
    public string? Country { get; set; }
    public ReleaseEvent[]? ReleaseEvents { get; set; }
    public LabelInfo[]? LabelInfo { get; set; }
    public string? Barcode { get; set; }
    public string? PackagingId { get; set; }
    public string? Packaging { get; set; }
    public string? StatusId { get; set; }
    public string? Status { get; set; }
    public string? Quality { get; set; }
    public TextRepresentation? TextRepresentation { get; set; }
    public string? Asin { get; set; }
    public Media[]? Media { get; set; }
    public CoverArtArchive? CoverArtArchive { get; set; }
}
