using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.Discogs.DiscogsApi;

public class Master
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("uri")]
    public string Uri { get; set; } = "";

    [JsonPropertyName("artists")]
    public List<ReleaseArtist> Artists { get; set; } = [];

    [JsonPropertyName("tracklist")]
    public List<ReleaseTrack> TrackList { get; set; } = [];

    [JsonPropertyName("year")]
    public int Year { get; set; }

    [JsonPropertyName("main_release")]
    public int MainRelease { get; set; }
}
