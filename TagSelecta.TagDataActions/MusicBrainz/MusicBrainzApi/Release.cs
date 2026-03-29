using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

public class Release
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("disambiguation")]
    public string? Disambiguation { get; set; }

    [JsonPropertyName("artist-credit")]
    public List<ArtistCredit>? ArtistCredit { get; set; }

    [JsonPropertyName("date")]
    public string? Date { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("release-events")]
    public List<ReleaseEvent>? ReleaseEvents { get; set; }

    [JsonPropertyName("label-info")]
    public List<LabelInfo>? LabelInfo { get; set; }

    [JsonPropertyName("barcode")]
    public string? Barcode { get; set; }

    [JsonPropertyName("packaging-id")]
    public string? PackagingId { get; set; }

    [JsonPropertyName("packaging")]
    public string? Packaging { get; set; }

    [JsonPropertyName("status-id")]
    public string? StatusId { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("quality")]
    public string? Quality { get; set; }

    [JsonPropertyName("text-representation")]
    public TextRepresentation? TextRepresentation { get; set; }

    [JsonPropertyName("asin")]
    public string? Asin { get; set; }

    [JsonPropertyName("media")]
    public List<Media>? Media { get; set; }

    [JsonPropertyName("cover-art-archive")]
    public CoverArtArchive? CoverArtArchive { get; set; }

    [JsonPropertyName("release-group")]
    public ReleaseGroup? ReleaseGroup { get; set; }
}
