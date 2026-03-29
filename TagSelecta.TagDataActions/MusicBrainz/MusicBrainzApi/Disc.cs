namespace TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

public class Disc
{
    public string? Id { get; set; }
    public int Sectors { get; set; }
    public List<int>? Offsets { get; set; }
    public int OffsetCount { get; set; }
}
