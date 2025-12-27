namespace TagSelecta.Cli.Commands.FixAlbum;

public class Album
{
    public required string Dir { get; set; }
    public required FixType FixType { get; set; }
    public required List<string> AlbumArtists { get; set; } = [];
    public required string AlbumName { get; set; }
    public required string Date { get; set; }
}