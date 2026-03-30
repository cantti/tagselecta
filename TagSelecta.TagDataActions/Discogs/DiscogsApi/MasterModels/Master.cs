using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.Discogs.DiscogsApi.MasterModels;

public class Master
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("data_quality")]
    public string? DataQuality { get; set; }

    [JsonPropertyName("uri")]
    public string? Uri { get; set; }

    [JsonPropertyName("resource_url")]
    public string? ResourceUrl { get; set; }

    [JsonPropertyName("artists")]
    public List<Artist>? Artists { get; set; }

    [JsonPropertyName("tracklist")]
    public List<Track>? TrackList { get; set; }

    [JsonPropertyName("styles")]
    public List<string>? Styles { get; set; }

    [JsonPropertyName("genres")]
    public List<string>? Genres { get; set; }

    [JsonPropertyName("videos")]
    public List<Video>? Videos { get; set; }

    [JsonPropertyName("year")]
    public int? Year { get; set; }

    [JsonPropertyName("main_release")]
    public int? MainRelease { get; set; }

    [JsonPropertyName("main_release_url")]
    public string? MainReleaseUrl { get; set; }

    [JsonPropertyName("versions_url")]
    public string? VersionsUrl { get; set; }

    [JsonPropertyName("images")]
    public List<Image>? Images { get; set; }

    [JsonPropertyName("num_for_sale")]
    public int? NumForSale { get; set; }

    [JsonPropertyName("lowest_price")]
    public decimal? LowestPrice { get; set; }
}
