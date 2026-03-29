using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

public class Artist
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("sort-name")]
    public string? SortName { get; set; }

    [JsonPropertyName("disambiguation")]
    public string? Disambiguation { get; set; }
}
