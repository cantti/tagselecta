using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

public class ReleaseGroup
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("primary-type")]
    public string? PrimaryType { get; set; }

    [JsonPropertyName("genres")]
    public List<Genre>? Genres { get; set; }
}
