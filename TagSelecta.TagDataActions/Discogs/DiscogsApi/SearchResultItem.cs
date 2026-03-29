using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.Discogs.DiscogsApi;

public class SearchResultItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("year")]
    public string Year { get; set; } = "";
}
