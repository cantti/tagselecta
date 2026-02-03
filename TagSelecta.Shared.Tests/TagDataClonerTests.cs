using TagLib;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.Shared.Tests;

public class TagDataClonerTests
{
    [Fact]
    public void Clone_CreatesDeepCopyOfTagData()
    {
        var original = new TagData
        {
            Album = "Test Album",
            // AlbumArtist = ["Artist1", "Artist2"],
            Artist = ["ArtistA"],
            Comment = "Some comment",
            Composer = ["Composer1"],
            Disc = "1",
            DiscTotal = "2",
            Genre = ["Genre1", "Genre2"],
            Title = "Test Title",
            Track = "5",
            TrackTotal = "10",
            Date = "2022",
            Label = "Test Label",
            CatalogNumber = "12345",
            Picture =
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
        };

        original.SetExtraField("key", "value");

        var clone = TagDataCloner.Clone(original);

        Assert.NotSame(original, clone);
        Assert.Equal(original.Album, clone.Album);
        Assert.Equal(original.AlbumArtist, clone.AlbumArtist);
        Assert.NotSame(original.AlbumArtist, clone.AlbumArtist);
        Assert.Equal(original.Picture.Count, clone.Picture.Count);
        Assert.NotSame(original.Picture[0], clone.Picture[0]);
        Assert.Equal(original.Picture[0].Data, clone.Picture[0].Data);
        Assert.Equal(original.Extra.Count, clone.Extra.Count);
        Assert.NotSame(original.Extra[0], clone.Extra[0]);
        Assert.Equal(original.Extra[0].Key, clone.Extra[0].Key);
        Assert.Equal(original.Extra[0].Text, clone.Extra[0].Text);
    }
}
