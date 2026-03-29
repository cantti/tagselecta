namespace TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

public class Recording
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? Disambiguation { get; set; }
    public int Length { get; set; }
    public bool Video { get; set; }
    public List<ArtistCredit>? ArtistCredit { get; set; }
}
