using TagSelecta.App.TagDataActions.AutoTrack;
using TagSelecta.Tagging;

namespace TagSelecta.Cli.Tests;

[Collection("Console")]
public class AutoTrackTests
{
    [Fact]
    public async Task AutoTrackTest()
    {
        // Arrange
        var action = new AutoTrackAction();

        var settings = new AutoTrackSettings { KeepDisk = true };

        var item1 = new IFileContext(
            "file1.mp3",
            new TagData()
            {
                Disc = "1",
                DiscTotal = "1",
                Track = "",
                TrackTotal = "",
            }
        );

        var item2 = new IFileContext(
            "file2.mp3",
            new TagData
            {
                Disc = "1",
                DiscTotal = "1",
                Track = "",
                TrackTotal = "",
            }
        );

        // Act
        await action.ProcessTagDataAsync(item1, [item1, item2], settings);
        await action.ProcessTagDataAsync(item2, [item1, item2], settings);

        // Assert
        Assert.Equal("1", item1.TagData.Track);
        Assert.Equal("2", item1.TagData.TrackTotal);
        Assert.Equal("1", item1.TagData.Disc);
        Assert.Equal("1", item1.TagData.DiscTotal);
        Assert.Equal("2", item2.TagData.Track);
        Assert.Equal("2", item2.TagData.TrackTotal);
        Assert.Equal("1", item2.TagData.Disc);
        Assert.Equal("1", item2.TagData.DiscTotal);
    }
}
