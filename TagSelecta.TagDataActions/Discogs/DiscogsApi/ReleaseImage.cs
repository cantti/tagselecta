using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.Discogs.DiscogsApi;

public class ReleaseImage
{
    [JsonPropertyName("uri")]
    public string Uri { get; set; } = "";
}
