using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

public class CoverArtArchive
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("artwork")]
    public bool Artwork { get; set; }

    [JsonPropertyName("front")]
    public bool Front { get; set; }

    [JsonPropertyName("back")]
    public bool Back { get; set; }

    [JsonPropertyName("darkened")]
    public bool Darkened { get; set; }
}
