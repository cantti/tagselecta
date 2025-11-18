using TagLib;

namespace TagSelecta.Tagging.Tests;

public class TagDataClonerTests
{
    [Fact]
    public void Clone_CreatesDeepCopyOfTagData()
    {
        var original = new TagData
        {
            Album = "Test Album",
            AlbumArtists = ["Artist1", "Artist2"],
            Artists = ["ArtistA"],
            Comment = "Some comment",
            Composers = ["Composer1"],
            Disc = "1",
            DiscTotal = "2",
            Genres = ["Genre1", "Genre2"],
            Title = "Test Title",
            Track = "5",
            TrackTotal = "10",
            Date = "2022",
            Label = "Test Label",
            CatalogNumber = "12345",
            DiscogsReleaseId = "67890",
            Pictures =
            [
                new Picture
                {
                    Data = new ByteVector([1, 2, 3]),
                    Description = "desc",
                    Filename = "file.jpg",
                    MimeType = "image/jpeg",
                    Type = PictureType.FrontCover,
                },
            ],
            Custom = [new CustomField("key", "value")],
        };

        var clone = TagDataCloner.Clone(original);

        Assert.NotSame(original, clone);
        Assert.Equal(original.Album, clone.Album);
        Assert.Equal(original.AlbumArtists, clone.AlbumArtists);
        Assert.NotSame(original.AlbumArtists, clone.AlbumArtists);
        Assert.Equal(original.Pictures.Count, clone.Pictures.Count);
        Assert.NotSame(original.Pictures[0], clone.Pictures[0]);
        Assert.Equal(original.Pictures[0].Data, clone.Pictures[0].Data);
        Assert.Equal(original.Custom.Count, clone.Custom.Count);
        Assert.NotSame(original.Custom[0], clone.Custom[0]);
        Assert.Equal(original.Custom[0].Key, clone.Custom[0].Key);
        Assert.Equal(original.Custom[0].Text, clone.Custom[0].Text);
    }
}
