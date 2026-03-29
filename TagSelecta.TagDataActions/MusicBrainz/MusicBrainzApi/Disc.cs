using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

public class Disc
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("sectors")]
    public int Sectors { get; set; }

    [JsonPropertyName("offsets")]
    public List<int>? Offsets { get; set; }

    [JsonPropertyName("offset-count")]
    public int OffsetCount { get; set; }
}
