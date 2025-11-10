namespace TagSelecta.Commands.Discogs;

public class Release
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Uri { get; set; } = "";
    public List<ReleaseArtist> Artists { get; set; } = [];
    public List<ReleaseTrack> TrackList { get; set; } = [];
    public List<string> Genres { get; set; } = [];
    public List<string> Styles { get; set; } = [];
    public List<ReleaseLabel> Labels { get; set; } = [];
    public List<ReleaseImage> Images { get; set; } = [];
    public uint Year { get; set; }
}
