using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.Discogs.DiscogsApi.MasterModels;

public class Image
{
    [JsonPropertyName("height")]
    public int? Height { get; set; }

    [JsonPropertyName("resource_url")]
    public string? ResourceUrl { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("uri")]
    public string? Uri { get; set; }

    [JsonPropertyName("uri150")]
    public string? Uri150 { get; set; }

    [JsonPropertyName("width")]
    public int? Width { get; set; }
}
