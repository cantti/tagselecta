using System.Text.Json.Serialization;

namespace TagSelecta.Commands.Discogs;

public class Master
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Uri { get; set; } = "";
    public List<ReleaseArtist> Artists { get; set; } = [];
    public List<ReleaseTrack> TrackList { get; set; } = [];
    public int Year { get; set; }

    [JsonPropertyName("main_release")]
    public int MainRelease { get; set; }
}
