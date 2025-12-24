using TagSelecta.Cli.Commands.TagDataCommands;
using TagSelecta.Tagging;

namespace TagSelecta.Cli.Tests;

[Collection("Console")]
public class SplitTests
{
    [Fact]
    public async Task SplitTest()
    {
        // Arrange
        var action = new SplitAction();

        var settings = new SplitSettings();

        var tagData = new TagData() { Artist = ["Artist1; Artist2"] };

        var context = new TagDataActionContext<SplitSettings>
        {
            Files = ["file1.mp3"],
            Settings = settings,
        };

        // Act
        context.SetCurrentFile(context.Files[0], 0, tagData);
        await action.ProcessTagDataAsync(context);

        // Assert
        Assert.Equal("Artist1", tagData.Artist[0]);
        Assert.Equal("Artist2", tagData.Artist[1]);
    }
}
