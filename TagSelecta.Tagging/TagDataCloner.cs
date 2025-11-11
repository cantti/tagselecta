namespace TagSelecta.Tagging;

public class TagDataCloner
{
    public static TagData Clone(TagData tagData)
    {
        var clone = new TagData
        {
            Album = tagData.Album,
            AlbumArtists = [.. tagData.AlbumArtists],
            Artists = [.. tagData.Artists],
            Comment = tagData.Comment,
            Composers = [.. tagData.Composers],
            Disc = tagData.Disc,
            DiscTotal = tagData.DiscTotal,
            Genres = [.. tagData.Genres],
            Title = tagData.Title,
            Track = tagData.Track,
            TrackTotal = tagData.TrackTotal,
            Year = tagData.Year,
            Pictures =
            [
                .. tagData.Pictures.Select(x => new TagLib.Picture
                {
                    Data = x.Data.ToArray(),
                    Description = x.Description,
                    Filename = x.Filename,
                    MimeType = x.MimeType,
                    Type = x.Type,
                }),
            ],
            Custom = new Dictionary<string, string>(tagData.Custom),
            Path = tagData.Path,
        };
        return clone;
    }
}
