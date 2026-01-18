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
        await action.ExecuteAsync(
            new TagDataActionExecuteContext<AutoTrackSettings>
            {
                Files = [item1, item2],
                Settings = settings,
                Target = item1,
            },
            CancellationToken.None
        );
        await action.ExecuteAsync(
            new TagDataActionExecuteContext<AutoTrackSettings>
            {
                Files = [item1, item2],
                Settings = settings,
                Target = item2,
            },
            CancellationToken.None
        );

        // Assert
        var currentTagData1 = item1.GetCurrentTagData();
        var currentTagData2 = item2.GetCurrentTagData();
        Assert.Equal("1", currentTagData1.Track);
        Assert.Equal("2", currentTagData1.TrackTotal);
        Assert.Equal("1", currentTagData1.Disc);
        Assert.Equal("1", currentTagData1.DiscTotal);
        Assert.Equal("2", currentTagData2.Track);
        Assert.Equal("2", currentTagData2.TrackTotal);
        Assert.Equal("1", currentTagData2.Disc);
        Assert.Equal("1", currentTagData2.DiscTotal);
    }
}
