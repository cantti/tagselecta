using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

public class LabelInfo
{
    [JsonPropertyName("catalog-number")]
    public string? CatalogNumber { get; set; }

    [JsonPropertyName("label")]
    public Label? Label { get; set; }
}
