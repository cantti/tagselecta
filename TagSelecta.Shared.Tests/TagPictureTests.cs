using TagLib;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.Shared.Tests;

public class TagPictureTests
{
    [Fact]
    public void Picture_CopiesMutableTagLibDataAndRoundTripsMetadata()
    {
        var source = new Picture
        {
            Data = new ByteVector(1, 2, 3),
            Description = "description",
            Filename = "cover.jpg",
            MimeType = "image/jpeg",
            Type = PictureType.BackCover,
        };
        var picture = new TagPicture(source);

        source.Data[0] = 9;
        source.Type = PictureType.FrontCover;
        source.Description = "changed";

        var converted = picture.ToTagLibPicture();
        Assert.Equal(new byte[] { 1, 2, 3 }, converted.Data.ToArray());
        Assert.Equal(PictureType.BackCover, converted.Type);
        Assert.Equal("description", converted.Description);
        Assert.Equal("cover.jpg", converted.Filename);
        Assert.Equal("image/jpeg", converted.MimeType);

        converted.Data[1] = 9;
        converted.Type = PictureType.FrontCover;
        Assert.False(picture.Data is byte[]);
        Assert.Throws<NotSupportedException>(() => ((IList<byte>)picture.Data)[0] = 9);
        Assert.Equal(new byte[] { 1, 2, 3 }, picture.Data.ToArray());
        Assert.Equal(PictureType.BackCover, picture.Type);
    }

    [Fact]
    public void PictureEq_ComparesMetadataAndBytes()
    {
        var original = new Picture
        {
            Data = new ByteVector(1, 2, 3),
            Type = PictureType.FrontCover,
            Description = "description",
            MimeType = "image/jpeg",
        };
        var picture = new TagPicture(original);
        Assert.True(TagDataComparer.PictureEq(picture, new TagPicture(original)));

        original.Data[0] = 9;
        Assert.False(TagDataComparer.PictureEq(picture, new TagPicture(original)));
        original.Data[0] = 1;
        original.Type = PictureType.BackCover;
        Assert.False(TagDataComparer.PictureEq(picture, new TagPicture(original)));
        original.Type = PictureType.FrontCover;
        original.Description = "other";
        Assert.False(TagDataComparer.PictureEq(picture, new TagPicture(original)));
        original.Description = "description";
        original.MimeType = "image/png";
        Assert.False(TagDataComparer.PictureEq(picture, new TagPicture(original)));
    }
}
