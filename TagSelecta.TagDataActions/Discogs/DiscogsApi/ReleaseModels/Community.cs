using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.Discogs.DiscogsApi.ReleaseModels;

public class Community
{
    [JsonPropertyName("contributors")]
    public List<CommunityUser>? Contributors { get; set; }

    [JsonPropertyName("data_quality")]
    public string? DataQuality { get; set; }

    [JsonPropertyName("have")]
    public int? Have { get; set; }

    [JsonPropertyName("rating")]
    public CommunityRating? Rating { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("submitter")]
    public CommunityUser? Submitter { get; set; }

    [JsonPropertyName("want")]
    public int? Want { get; set; }
}
