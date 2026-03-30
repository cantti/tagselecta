using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.Discogs.DiscogsApi.ReleaseModels;

public class CommunityRating
{
    [JsonPropertyName("average")]
    public double? Average { get; set; }

    [JsonPropertyName("count")]
    public int? Count { get; set; }
}
