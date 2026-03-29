namespace TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

public class Media
{
    public List<Disc>? Discs { get; set; }
    public int Position { get; set; }
    public string? Title { get; set; }
    public string? FormatId { get; set; }
    public string? Format { get; set; }
    public int TrackCount { get; set; }
    public int TrackOffset { get; set; }
    public List<Track>? Tracks { get; set; }
}
