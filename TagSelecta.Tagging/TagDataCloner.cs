namespace TagSelecta.Tagging;

public class TagDataCloner
{
    public static TagData Clone(TagData tagData)
    {
        var clone = new TagData
        {
            Album = tagData.Album,
            AlbumArtists = tagData.AlbumArtists.ToList(),
            Artists = tagData.Artists.ToList(),
            Comment = tagData.Comment,
            Composers = tagData.Composers.ToList(),
            Disc = tagData.Disc,
            DiscTotal = tagData.DiscTotal,
            Genres = tagData.Genres.ToList(),
            Title = tagData.Title,
            Track = tagData.Track,
            TrackTotal = tagData.TrackTotal,
            Year = tagData.Year,
            Label = tagData.Label,
            CatalogNumber = tagData.CatalogNumber,
            DiscogsReleaseId = tagData.DiscogsReleaseId,
            Pictures = tagData
                .Pictures.Select(x => new TagLib.Picture
                {
                    Data = x.Data.ToArray(),
                    Description = x.Description,
                    Filename = x.Filename,
                    MimeType = x.MimeType,
                    Type = x.Type,
                })
                .ToList(),
            Custom = tagData.Custom.Select(x => new CustomField(x.Key, x.Value)).ToList(),
        };
        return clone;
    }
}
