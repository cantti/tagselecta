using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.Discogs.DiscogsApi.ReleaseModels;

public class Release
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("data_quality")]
    public string? DataQuality { get; set; }

    [JsonPropertyName("thumb")]
    public string? Thumb { get; set; }

    [JsonPropertyName("uri")]
    public string? Uri { get; set; }

    [JsonPropertyName("resource_url")]
    public string? ResourceUrl { get; set; }

    [JsonPropertyName("artists")]
    public List<Artist>? Artists { get; set; }

    [JsonPropertyName("extraartists")]
    public List<Artist>? ExtraArtists { get; set; }

    [JsonPropertyName("community")]
    public Community? Community { get; set; }

    [JsonPropertyName("companies")]
    public List<Company>? Companies { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("date_added")]
    public DateTime? DateAdded { get; set; }

    [JsonPropertyName("date_changed")]
    public DateTime? DateChanged { get; set; }

    [JsonPropertyName("estimated_weight")]
    public int? EstimatedWeight { get; set; }

    [JsonPropertyName("tracklist")]
    public List<Track>? TrackList { get; set; }

    [JsonPropertyName("genres")]
    public List<string>? Genres { get; set; }

    [JsonPropertyName("styles")]
    public List<string>? Styles { get; set; }

    [JsonPropertyName("format_quantity")]
    public int? FormatQuantity { get; set; }

    [JsonPropertyName("formats")]
    public List<Format>? Formats { get; set; }

    [JsonPropertyName("identifiers")]
    public List<Identifier>? Identifiers { get; set; }

    [JsonPropertyName("labels")]
    public List<Label>? Labels { get; set; }

    [JsonPropertyName("images")]
    public List<Image>? Images { get; set; }

    [JsonPropertyName("lowest_price")]
    public decimal? LowestPrice { get; set; }

    [JsonPropertyName("master_id")]
    public int? MasterId { get; set; }

    [JsonPropertyName("master_url")]
    public string? MasterUrl { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("num_for_sale")]
    public int? NumForSale { get; set; }

    [JsonPropertyName("released")]
    public string? Released { get; set; }

    [JsonPropertyName("released_formatted")]
    public string? ReleasedFormatted { get; set; }

    [JsonPropertyName("series")]
    public List<Series>? Series { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("videos")]
    public List<Video>? Videos { get; set; }

    [JsonPropertyName("year")]
    public int? Year { get; set; }
}
