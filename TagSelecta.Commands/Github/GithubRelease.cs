using System.Text.Json.Serialization;

namespace TagSelecta.Commands.Github;

public class GithubRelease
{
    [JsonPropertyName("tag_name")]
    public string? TagName { get; set; }
}
