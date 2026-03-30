using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.Discogs.DiscogsApi.MasterModels;

public class Track
{
    [JsonPropertyName("duration")]
    public string? Duration { get; set; }

    [JsonPropertyName("position")]
    public string? Position { get; set; }

    [JsonPropertyName("type_")]
    public string? Type { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("extraartists")]
    public List<Artist>? ExtraArtists { get; set; }
}
