using TagSelecta.Shared.TagDataActions;
using TagSelecta.Shared.Tagging;
using TagSelecta.Shared.TrackedFiles;
using TagSelecta.TagDataActions.Edit;

namespace TagSelecta.TagDataActions.Tests;

public class EditTests
{
    [Fact]
    public async Task EditTest()
    {
        // Arrange
        var action = new EditAction();

        var settings = new EditSettings
        {
            Album = "Test Album",
            AlbumArtist = "Test Album Artist",
            Artist = "Test Artist",
            Bpm = "120",
            CatalogNumber = "TEST-001",
            Comment = "Test comment",
            Composer = "Test Composer",
            Conductor = "Test Conductor",
            Copyright = "Test Copyright",
            Date = "2025",
            Disc = "1",
            DiscTotal = "2",
            Genre = "Test Genre",
            Isrc = "TESTISRC123",
            Label = "Test Label",
            Publisher = "Test Publisher",
            Title = "Test Title",
            Track = "5",
            TrackTotal = "12",
            Set = ["test_field=test_value"],
        };

        var tagData = new TagData
        {
            Album = "Original Album",
            AlbumArtist = ["Original Album Artist"],
            Artist = ["Original Artist"],
            Bpm = "90",
            CatalogNumber = "ORIG-999",
            Comment = "Original comment",
            Composer = ["Original Composer"],
            Conductor = "Original Conductor",
            Copyright = "Original Copyright",
            Date = "2000",
            Disc = "1",
            DiscTotal = "1",
            Genre = ["Original Genre"],
            Isrc = "ORIGINALISRC",
            Label = "Original Label",
            Publisher = "Original Publisher",
            Title = "Original Title",
            Track = "1",
            TrackTotal = "10",
            Picture = [],
        };

        tagData.SetCustomField("original_field", "original_value");

        var item = new TrackedFile("file.mp3", tagData);

        // Act
        await action.ExecuteAsync(
            new TagDataActionExecuteContext<EditSettings>
            {
                Files = [item],
                Settings = settings,
                Target = item,
            },
            CancellationToken.None
        );

        // Assert
        var currentTagData = item.GetCurrentTagData();
        Assert.Equal(settings.Album, currentTagData.Album);
        Assert.Equal(settings.AlbumArtist.ToMulti(), currentTagData.AlbumArtist);
        Assert.Equal(settings.Artist.ToMulti(), currentTagData.Artist);
        Assert.Equal(settings.Bpm, currentTagData.Bpm);
        Assert.Equal(settings.CatalogNumber, currentTagData.CatalogNumber);
        Assert.Equal(settings.Comment, currentTagData.Comment);
        Assert.Equal(settings.Composer.ToMulti(), currentTagData.Composer);
        Assert.Equal(settings.Conductor, currentTagData.Conductor);
        Assert.Equal(settings.Copyright, currentTagData.Copyright);
        Assert.Equal(settings.Date, currentTagData.Date);
        Assert.Equal(settings.Disc, currentTagData.Disc);
        Assert.Equal(settings.DiscTotal, currentTagData.DiscTotal);
        Assert.Equal(settings.Genre.ToMulti(), currentTagData.Genre);
        Assert.Equal(settings.Isrc, currentTagData.Isrc);
        Assert.Equal(settings.Label, currentTagData.Label);
        Assert.Equal(settings.Publisher, currentTagData.Publisher);
        Assert.Equal(settings.Title, currentTagData.Title);
        Assert.Equal(settings.Track, currentTagData.Track);
        Assert.Equal(settings.TrackTotal, currentTagData.TrackTotal);

        // assert custom
        Assert.Equal(2, currentTagData.Custom.Count);
        Assert.Equal("original_field", currentTagData.Custom[0].Key);
        Assert.Equal("original_value", currentTagData.Custom[0].Text);
        Assert.Equal("test_field", currentTagData.Custom[1].Key);
        Assert.Equal("test_value", currentTagData.Custom[1].Text);
    }
}
