namespace TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

public class Track
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public int Length { get; set; }
    public string? Number { get; set; }
    public int Position { get; set; }
    public ArtistCredit[]? ArtistCredit { get; set; }
    public Recording? Recording { get; set; }
}
