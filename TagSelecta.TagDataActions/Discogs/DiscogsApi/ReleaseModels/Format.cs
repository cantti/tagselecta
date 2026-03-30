using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.Discogs.DiscogsApi.ReleaseModels;

public class Format
{
    [JsonPropertyName("descriptions")]
    public List<string>? Descriptions { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("qty")]
    public string? Qty { get; set; }
}
