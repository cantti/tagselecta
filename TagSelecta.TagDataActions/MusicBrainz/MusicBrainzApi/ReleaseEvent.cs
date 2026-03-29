using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

public class ReleaseEvent
{
    [JsonPropertyName("date")]
    public string? Date { get; set; }

    [JsonPropertyName("area")]
    public Area? Area { get; set; }
}
