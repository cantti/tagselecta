namespace TagSelecta.Shared.Tagging;

public static class FieldName
{
    public const string Album = "album";
    public const string AlbumArtist = "albumartist";
    public const string Artist = "artist";
    public const string Bpm = "bpm";
    public const string Comment = "comment";
    public const string Composer = "composer";
    public const string Conductor = "conductor";
    public const string Copyright = "copyright";
    public const string Date = "date";
    public const string DiscNumber = "discnumber";
    public const string DiscTotal = "disctotal";
    public const string Genre = "genre";
    public const string Isrc = "isrc";
    public const string Publisher = "publisher";
    public const string Title = "title";
    public const string TrackNumber = "tracknumber";
    public const string TrackTotal = "tracktotal";

    private static readonly string[] _all =
    [
        Album,
        AlbumArtist,
        Artist,
        Bpm,
        Comment,
        Composer,
        Conductor,
        Copyright,
        Date,
        DiscNumber,
        DiscTotal,
        Genre,
        Isrc,
        Publisher,
        Title,
        TrackNumber,
        TrackTotal,
    ];

    public static IReadOnlyList<string> All()
    {
        return _all;
    }
}
