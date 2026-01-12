using TagSelecta.Shared.TagDataActions;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Edit;

namespace TagSelecta.Cli.Tests;

[Collection("Console")]
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

        var item = new TagDataOperation("file.mp3", tagData);

        // Act
        await action.ExecuteAsync(item, [item], settings, CancellationToken.None);

        // Assert
        Assert.Equal(settings.Album, tagData.Album);
        Assert.Equal(settings.AlbumArtist.ToMulti(), tagData.AlbumArtist);
        Assert.Equal(settings.Artist.ToMulti(), tagData.Artist);
        Assert.Equal(settings.Bpm, tagData.Bpm);
        Assert.Equal(settings.CatalogNumber, tagData.CatalogNumber);
        Assert.Equal(settings.Comment, tagData.Comment);
        Assert.Equal(settings.Composer.ToMulti(), tagData.Composer);
        Assert.Equal(settings.Conductor, tagData.Conductor);
        Assert.Equal(settings.Copyright, tagData.Copyright);
        Assert.Equal(settings.Date, tagData.Date);
        Assert.Equal(settings.Disc, tagData.Disc);
        Assert.Equal(settings.DiscTotal, tagData.DiscTotal);
        Assert.Equal(settings.Genre.ToMulti(), tagData.Genre);
        Assert.Equal(settings.Isrc, tagData.Isrc);
        Assert.Equal(settings.Label, tagData.Label);
        Assert.Equal(settings.Publisher, tagData.Publisher);
        Assert.Equal(settings.Title, tagData.Title);
        Assert.Equal(settings.Track, tagData.Track);
        Assert.Equal(settings.TrackTotal, tagData.TrackTotal);

        // assert custom
        Assert.Equal(2, tagData.Custom.Count);
        Assert.Equal("original_field", tagData.Custom[0].Key);
        Assert.Equal("original_value", tagData.Custom[0].Text);
        Assert.Equal("test_field", tagData.Custom[1].Key);
        Assert.Equal("test_value", tagData.Custom[1].Text);
    }
}
