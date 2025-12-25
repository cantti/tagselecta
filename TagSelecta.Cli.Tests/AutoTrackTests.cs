using TagSelecta.Cli.Commands.TagDataCommands;
using TagSelecta.Cli.Commands.TagDataCommands.Common;
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

        TagData[] tagDataList =
        [
            new()
            {
                Disc = "1",
                DiscTotal = "1",
                Track = "",
                TrackTotal = "",
            },
            new()
            {
                Disc = "1",
                DiscTotal = "1",
                Track = "",
                TrackTotal = "",
            },
        ];

        var context = new TagDataActionContext<AutoTrackSettings>
        {
            Files = ["file1.mp3", "file2.mp3"],
            Settings = settings,
        };

        // Act
        context.SetCurrentFile(context.Files[0], 0, tagDataList[0]);
        await action.ProcessTagDataAsync(context);

        context.SetCurrentFile(context.Files[1], 1, tagDataList[1]);
        await action.ProcessTagDataAsync(context);

        // Assert
        Assert.Equal("1", tagDataList[0].Track);
        Assert.Equal("2", tagDataList[0].TrackTotal);
        Assert.Equal("1", tagDataList[0].Disc);
        Assert.Equal("1", tagDataList[0].DiscTotal);
        Assert.Equal("2", tagDataList[1].Track);
        Assert.Equal("2", tagDataList[1].TrackTotal);
        Assert.Equal("1", tagDataList[1].Disc);
        Assert.Equal("1", tagDataList[1].DiscTotal);
    }
}
