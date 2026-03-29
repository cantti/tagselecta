using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.Discogs.DiscogsApi;

public class SearchResult
{
    [JsonPropertyName("results")]
    public List<SearchResultItem> Results { get; set; } = [];
}
