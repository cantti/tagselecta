using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

public class Area
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("sort-name")]
    public string? SortName { get; set; }

    [JsonPropertyName("iso-3166-1-codes")]
    public List<string>? Iso31661Codes { get; set; }

    [JsonPropertyName("disambiguation")]
    public string? Disambiguation { get; set; }
}
