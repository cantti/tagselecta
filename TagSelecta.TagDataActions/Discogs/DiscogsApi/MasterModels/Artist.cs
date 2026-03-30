using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.Discogs.DiscogsApi.MasterModels;

public class Artist
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("anv")]
    public string? Anv { get; set; }

    [JsonPropertyName("join")]
    public string? Join { get; set; }

    [JsonPropertyName("role")]
    public string? Role { get; set; }

    [JsonPropertyName("tracks")]
    public string? Tracks { get; set; }

    [JsonPropertyName("resource_url")]
    public string? ResourceUrl { get; set; }
}
