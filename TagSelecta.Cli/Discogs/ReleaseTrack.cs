using System.Text.Json.Serialization;

namespace TagSelecta.Cli.Discogs;

public class ReleaseTrack
{
    public string Title { get; set; } = "";

    [JsonPropertyName("type_")]
    public string Type { get; set; } = "";
    public string Duration { get; set; } = "";
    public string Position { get; set; } = "";
    public List<ReleaseArtist> Artists { get; set; } = [];
}
