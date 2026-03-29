namespace TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

public class Media
{
    public Disc[]? Discs { get; set; }
    public int Position { get; set; }
    public string? Title { get; set; }
    public string? FormatId { get; set; }
    public string? Format { get; set; }
    public int TrackCount { get; set; }
    public int TrackOffset { get; set; }
    public Track[]? Tracks { get; set; }
}
