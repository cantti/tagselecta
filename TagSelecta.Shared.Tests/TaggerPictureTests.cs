using TagLib;
using TagSelecta.Shared.Tagging;
using File = System.IO.File;

namespace TagSelecta.Shared.Tests;

public class TaggerPictureTests
{
    [Fact]
    public void WriteAndRead_PreservesPicturesAndTagEdits()
    {
        var file = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.mp3");
        File.Copy(Path.Combine(AppContext.BaseDirectory, "TestData", "song.mp3"), file);
        try
        {
            var tagger = new Tagger(new TaggerConfig());
            var tagData = tagger.ReadTags(file);
            var bytes = new byte[] { 0xff, 0xd8, 0xff, 0xdb, 1, 2, 3 };
            tagData.Picture =
            [
                new TagPicture(
                    new Picture
                    {
                        Data = new ByteVector(bytes),
                        Type = PictureType.BackCover,
                        MimeType = "image/jpeg",
                        Description = "back cover",
                    }
                ),
            ];
            tagData.SetValue(FieldName.Title, "Updated title");

            tagger.WriteTags(file, tagData);

            var result = tagger.ReadTags(file);
            var picture = Assert.Single(result.Picture);
            Assert.Equal(bytes, picture.Data.ToArray());
            Assert.Equal(PictureType.BackCover, picture.Type);
            Assert.Equal("image/jpeg", picture.MimeType);
            Assert.Equal("back cover", picture.Description);
            Assert.Equal(["Updated title"], result.GetValue(FieldName.Title));
        }
        finally
        {
            File.Delete(file);
        }
    }
}
