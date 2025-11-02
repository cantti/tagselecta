namespace TagSelecta.Discogs;

public class ReleaseTrack
{
    public string Title { get; set; } = "";
    public string Duration { get; set; } = "";
    public string Position { get; set; } = "";
    public List<ReleaseArtist> Artists { get; set; } = [];
}
