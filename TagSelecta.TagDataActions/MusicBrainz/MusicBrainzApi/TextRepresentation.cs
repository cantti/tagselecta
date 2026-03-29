using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

public class TextRepresentation
{
    [JsonPropertyName("language")]
    public string? Language { get; set; }

    [JsonPropertyName("script")]
    public string? Script { get; set; }
}
