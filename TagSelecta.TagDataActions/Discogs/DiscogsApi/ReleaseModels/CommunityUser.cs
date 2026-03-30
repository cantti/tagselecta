using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.Discogs.DiscogsApi.ReleaseModels;

public class CommunityUser
{
    [JsonPropertyName("resource_url")]
    public string? ResourceUrl { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }
}
