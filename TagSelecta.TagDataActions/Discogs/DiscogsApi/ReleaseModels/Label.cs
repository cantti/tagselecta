using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.Discogs.DiscogsApi.ReleaseModels;

public class Label
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("catno")]
    public string? CatNo { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("entity_type")]
    public string? EntityType { get; set; }

    [JsonPropertyName("resource_url")]
    public string? ResourceUrl { get; set; }
}
