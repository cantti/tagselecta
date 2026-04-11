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
            Picture =
            [
                new Picture
                {
                    Data = new ByteVector(1, 2, 3),
                    Description = "desc",
                    Filename = "file.jpg",
                    MimeType = "image/jpeg",
                    Type = PictureType.FrontCover,
                },
            ],
        };

        original.SetValue(FieldName.Album, "Test Album");
        original.SetValue(FieldName.AlbumArtist, ["Artist1", "Artist2"]);
        original.SetValue(FieldName.Artist, ["ArtistA"]);
        original.SetValue(FieldName.Comment, "Some comment");
        original.SetValue(FieldName.Composer, ["Composer1"]);
        original.SetValue(FieldName.DiscNumber, "1");
        original.SetValue(FieldName.DiscTotal, "2");
        original.SetValue(FieldName.Genre, ["Genre1", "Genre2"]);
        original.SetValue(FieldName.Title, "Test Title");
        original.SetValue(FieldName.TrackNumber, "5");
        original.SetValue(FieldName.TrackTotal, "10");
        original.SetValue(FieldName.Date, "2022");
        original.SetValue("label", "Test Label");
        original.SetValue("catalognumber", "12345");

        var clone = TagDataCloner.Clone(original);

        Assert.NotSame(original, clone);
        var expectedKeys = new[]
        {
            FieldName.Album,
            FieldName.AlbumArtist,
            FieldName.Artist,
            FieldName.Comment,
            FieldName.Composer,
            FieldName.Date,
            FieldName.DiscNumber,
            FieldName.DiscTotal,
            FieldName.Genre,
            FieldName.Title,
            FieldName.TrackNumber,
            FieldName.TrackTotal,
            "label",
            "catalognumber",
        };

        foreach (var key in expectedKeys)
        {
            Assert.Equal(original.GetValue(key), clone.GetValue(key));
            Assert.NotSame(original.GetValue(key), clone.GetValue(key));
        }

        Assert.Equal(original.Picture.Count, clone.Picture.Count);
        Assert.NotSame(original.Picture[0], clone.Picture[0]);
        Assert.Equal(original.Picture[0].Data, clone.Picture[0].Data);
        Assert.Equal(original.Fields.Count, clone.Fields.Count);

        for (var i = 0; i < original.Fields.Count; i++)
        {
            Assert.NotSame(original.Fields[i], clone.Fields[i]);
            Assert.Equal(original.Fields[i].Key, clone.Fields[i].Key);
            Assert.Equal(original.Fields[i].Text, clone.Fields[i].Text);
            Assert.NotSame(original.Fields[i].Text, clone.Fields[i].Text);
        }
    }
}
