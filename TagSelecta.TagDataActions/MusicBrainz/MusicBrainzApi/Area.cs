using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

public class Area
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? SortName { get; set; }

    [JsonPropertyName("iso-3166-1-codes")]
    public string[]? Iso31661Codes { get; set; }
    public string? Disambiguation { get; set; }
}
