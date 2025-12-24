using TagSelecta.Cli.Commands.TagDataCommands;
using TagSelecta.Cli.Tests.Utils;
using TagSelecta.Shared;
using TagSelecta.Tagging;

namespace TagSelecta.Cli.Tests;

[Collection("Console")]
public class WriteTests
{
    [Fact]
    public void WriteCliTest()
    {
        var app = CommandAppFactory.CreateTestApp<TagDataCommand<WriteSettings>>();
        app.Console.Input.PushTextWithEnter("y");

        var result = app.Run("./TestData/WriteTest/01 Song 1.mp3", "-t", "New Song 1");

        Assert.Contains("Status: success", result.Output);
    }

    [Fact]
    public async Task WriteTest()
    {
        // Arrange
        var action = new WriteAction();

        var settings = new WriteSettings
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
            DiscogsReleaseId = "123456",
            Genre = "Test Genre",
            Isrc = "TESTISRC123",
            Label = "Test Label",
            Publisher = "Test Publisher",
            Title = "Test Title",
            Track = "5",
            TrackTotal = "12",
            Custom = ["test_field=test_value"],
        };

        var originalTagData = new TagData
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
            DiscogsReleaseId = "000000",
            Genre = ["Original Genre"],
            Isrc = "ORIGINALISRC",
            Label = "Original Label",
            Publisher = "Original Publisher",
            Title = "Original Title",
            Track = "1",
            TrackTotal = "10",
            Picture = [],
            Custom = [new("original_field", "original_value")],
        };

        var context = new TagDataActionContext<WriteSettings>
        {
            Files = ["file1.mp3"],
            Settings = settings,
        };

        context.SetCurrentFile("file1.mp3", 0, originalTagData);

        // Act
        await action.ProcessTagDataAsync(context);

        var result = context.TagData;

        Assert.Equal(settings.Album, result.Album);
        Assert.Equal(settings.AlbumArtist.ToMulti(), result.AlbumArtist);
        Assert.Equal(settings.Artist.ToMulti(), result.Artist);
        Assert.Equal(settings.Bpm, result.Bpm);
        Assert.Equal(settings.CatalogNumber, result.CatalogNumber);
        Assert.Equal(settings.Comment, result.Comment);
        Assert.Equal(settings.Composer.ToMulti(), result.Composer);
        Assert.Equal(settings.Conductor, result.Conductor);
        Assert.Equal(settings.Copyright, result.Copyright);
        Assert.Equal(settings.Date, result.Date);
        Assert.Equal(settings.Disc, result.Disc);
        Assert.Equal(settings.DiscTotal, result.DiscTotal);
        Assert.Equal(settings.DiscogsReleaseId, result.DiscogsReleaseId);
        Assert.Equal(settings.Genre.ToMulti(), result.Genre);
        Assert.Equal(settings.Isrc, result.Isrc);
        Assert.Equal(settings.Label, result.Label);
        Assert.Equal(settings.Publisher, result.Publisher);
        Assert.Equal(settings.Title, result.Title);
        Assert.Equal(settings.Track, result.Track);
        Assert.Equal(settings.TrackTotal, result.TrackTotal);

        // assert custom
        Assert.Equal(2, result.Custom.Count);
        Assert.Equal("original_field", result.Custom[0].Key);
        Assert.Equal("original_value", result.Custom[0].Text);
        Assert.Equal("test_field", result.Custom[1].Key);
        Assert.Equal("test_value", result.Custom[1].Text);
    }
}
