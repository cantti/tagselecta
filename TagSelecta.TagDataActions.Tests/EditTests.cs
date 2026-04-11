using NSubstitute;
using TagSelecta.Shared.Http;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;
using TagSelecta.TagDataActions.Edit;
using TagSelecta.TagDataActions.Tests.Utils;

namespace TagSelecta.TagDataActions.Tests;

public class EditTests
{
    [Fact]
    public async Task EditTest()
    {
        // Arrange
        var downloader = Substitute.For<IDownloader>();

        ITagDataAction action = new EditAction(downloader);

        var settings = new EditSettings
        {
            Album = "Test Album",
            AlbumArtist = "Test Album Artist",
            Artist = "Test Artist",
            Bpm = "120",
            Comment = "Test comment",
            Composer = "Test Composer",
            Conductor = "Test Conductor",
            Copyright = "Test Copyright",
            Date = "2025",
            Disc = "1",
            DiscTotal = "2",
            Genre = "Test Genre",
            Isrc = "TESTISRC123",
            Publisher = "Test Publisher",
            Title = "Test Title",
            Track = "5",
            TrackTotal = "12",
            Key = ["test_field"],
            Value = ["test_value"],
        };

        var tagData = new TagData { Picture = [] };
        tagData.SetValue(FieldName.Album, "Original Album");
        tagData.SetValue(FieldName.AlbumArtist, ["Original Album Artist"]);
        tagData.SetValue(FieldName.Artist, ["Original Artist"]);
        tagData.SetValue(FieldName.Bpm, "90");
        tagData.SetValue(FieldName.Comment, "Original comment");
        tagData.SetValue(FieldName.Composer, ["Original Composer"]);
        tagData.SetValue(FieldName.Conductor, "Original Conductor");
        tagData.SetValue(FieldName.Copyright, "Original Copyright");
        tagData.SetValue(FieldName.Date, "2000");
        tagData.SetValue(FieldName.Disc, "1");
        tagData.SetValue(FieldName.DiscTotal, "1");
        tagData.SetValue(FieldName.Genre, ["Original Genre"]);
        tagData.SetValue(FieldName.Isrc, "ORIGINALISRC");
        tagData.SetValue(FieldName.Publisher, "Original Publisher");
        tagData.SetValue(FieldName.Title, "Original Title");
        tagData.SetValue(FieldName.Track, "1");
        tagData.SetValue(FieldName.TrackTotal, "10");
        tagData.SetValue("label", "Original Label");
        tagData.SetValue("catalognumber", "Original Catalog Number");

        var item = new TestTarget("file.mp3", tagData);

        // Act
        await action.Execute(
            new TagDataActionExecuteContext { Settings = settings, Target = item },
            CancellationToken.None
        );

        // Assert
        var currentTagData = item.CurrentTagData;
        Assert.Equal(settings.Album, currentTagData.GetValueFirst(FieldName.Album));
        Assert.Equal(settings.AlbumArtist!.SplitTagValues(), currentTagData.GetValue(FieldName.AlbumArtist));
        Assert.Equal(settings.Artist!.SplitTagValues(), currentTagData.GetValue(FieldName.Artist));
        Assert.Equal(settings.Bpm, currentTagData.GetValueFirst(FieldName.Bpm));
        Assert.Equal(settings.Comment, currentTagData.GetValueFirst(FieldName.Comment));
        Assert.Equal(settings.Composer!.SplitTagValues(), currentTagData.GetValue(FieldName.Composer));
        Assert.Equal(settings.Conductor, currentTagData.GetValueFirst(FieldName.Conductor));
        Assert.Equal(settings.Copyright, currentTagData.GetValueFirst(FieldName.Copyright));
        Assert.Equal(settings.Date, currentTagData.GetValueFirst(FieldName.Date));
        Assert.Equal(settings.Disc, currentTagData.GetValueFirst(FieldName.Disc));
        Assert.Equal(settings.DiscTotal, currentTagData.GetValueFirst(FieldName.DiscTotal));
        Assert.Equal(settings.Genre!.SplitTagValues(), currentTagData.GetValue(FieldName.Genre));
        Assert.Equal(settings.Isrc, currentTagData.GetValueFirst(FieldName.Isrc));
        Assert.Equal(settings.Publisher, currentTagData.GetValueFirst(FieldName.Publisher));
        Assert.Equal(settings.Title, currentTagData.GetValueFirst(FieldName.Title));
        Assert.Equal(settings.Track, currentTagData.GetValueFirst(FieldName.Track));
        Assert.Equal(settings.TrackTotal, currentTagData.GetValueFirst(FieldName.TrackTotal));
        Assert.Equal(settings.Value[0], currentTagData.GetValueFirst(settings.Key[0]));
        Assert.Equal("Original Label", currentTagData.GetValueFirst("label"));
        Assert.Equal("Original Catalog Number", currentTagData.GetValueFirst("catalognumber"));
    }
}
