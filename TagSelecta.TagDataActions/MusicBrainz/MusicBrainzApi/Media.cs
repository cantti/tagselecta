using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

public class Media
{
    [JsonPropertyName("discs")]
    public List<Disc>? Discs { get; set; }

    [JsonPropertyName("position")]
    public int Position { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("format-id")]
    public string? FormatId { get; set; }

    [JsonPropertyName("format")]
    public string? Format { get; set; }

    [JsonPropertyName("track-count")]
    public int TrackCount { get; set; }

    [JsonPropertyName("track-offset")]
    public int TrackOffset { get; set; }

    [JsonPropertyName("tracks")]
    public List<Track>? Tracks { get; set; }
}
