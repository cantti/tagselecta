using TagSelecta.Shared.TagDataActions;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.AutoTrack;

namespace TagSelecta.TagDataActions.Tests;

public class AutoTrackTests
{
    [Fact]
    public async Task AutoTrackTest()
    {
        // Arrange
        var action = new AutoTrackAction();

        var settings = new AutoTrackSettings { KeepDisk = true };

        var item1 = new TagDataOperation(
            "file1.mp3",
            new TagData()
            {
                Disc = "1",
                DiscTotal = "1",
                Track = "",
                TrackTotal = "",
            }
        );

        var item2 = new TagDataOperation(
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
        await action.ExecuteAsync(item1, [item1, item2], settings, CancellationToken.None);
        await action.ExecuteAsync(item2, [item1, item2], settings, CancellationToken.None);

        // Assert
        Assert.Equal("1", item1.CurrentTagData.Track);
        Assert.Equal("2", item1.CurrentTagData.TrackTotal);
        Assert.Equal("1", item1.CurrentTagData.Disc);
        Assert.Equal("1", item1.CurrentTagData.DiscTotal);
        Assert.Equal("2", item2.CurrentTagData.Track);
        Assert.Equal("2", item2.CurrentTagData.TrackTotal);
        Assert.Equal("1", item2.CurrentTagData.Disc);
        Assert.Equal("1", item2.CurrentTagData.DiscTotal);
    }
}
