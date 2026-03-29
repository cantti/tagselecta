using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.Discogs.DiscogsApi;

public class Release
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("uri")]
    public string? Uri { get; set; }

    [JsonPropertyName("artists")]
    public List<ReleaseArtist>? Artists { get; set; }

    [JsonPropertyName("tracklist")]
    public List<ReleaseTrack>? TrackList { get; set; }

    [JsonPropertyName("genres")]
    public List<string>? Genres { get; set; }

    [JsonPropertyName("styles")]
    public List<string>? Styles { get; set; }

    [JsonPropertyName("labels")]
    public List<ReleaseLabel>? Labels { get; set; }

    [JsonPropertyName("images")]
    public List<ReleaseImage>? Images { get; set; }

    [JsonPropertyName("year")]
    public int Year { get; set; }
}
