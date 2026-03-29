using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.Discogs.DiscogsApi;

public class MasterVersionList
{
    [JsonPropertyName("versions")]
    public List<MasterVersion> Versions { get; set; } = [];
}
