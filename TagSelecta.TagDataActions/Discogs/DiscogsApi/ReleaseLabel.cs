using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.Discogs.DiscogsApi;

public class ReleaseLabel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("catno")]
    public string CatNo { get; set; } = "";

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
}
