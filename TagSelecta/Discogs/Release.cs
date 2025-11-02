namespace TagSelecta.Discogs;

public class Release
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Uri { get; set; } = "";
    public List<ReleaseArtist> Artists { get; set; } = [];
    public List<ReleaseTrack> TrackList { get; set; } = [];
    public int Year { get; set; }
}
