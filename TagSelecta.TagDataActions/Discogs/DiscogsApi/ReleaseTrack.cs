using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.Discogs.DiscogsApi;

public class ReleaseTrack
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("type_")]
    public string? Type { get; set; }

    [JsonPropertyName("duration")]
    public string? Duration { get; set; }

    [JsonPropertyName("position")]
    public string? Position { get; set; }

    [JsonPropertyName("artists")]
    public List<ReleaseArtist>? Artists { get; set; }
}
