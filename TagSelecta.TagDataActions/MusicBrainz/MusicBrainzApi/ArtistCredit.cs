using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

public class ArtistCredit
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("joinphrase")]
    public string? Joinphrase { get; set; }

    [JsonPropertyName("artist")]
    public Artist? Artist { get; set; }
}
