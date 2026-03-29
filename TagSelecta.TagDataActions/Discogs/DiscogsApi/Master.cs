using System.Text.Json.Serialization;

namespace TagSelecta.TagDataActions.Discogs.DiscogsApi;

public class Master
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Uri { get; set; } = "";
    public List<ReleaseArtist> Artists { get; set; } = [];
    public List<ReleaseTrack> Tracklist { get; set; } = [];
    public int Year { get; set; }
    public int MainRelease { get; set; }
}
