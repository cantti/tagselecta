using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.Discogs.DiscogsApi;

public class MasterVersion
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
}
