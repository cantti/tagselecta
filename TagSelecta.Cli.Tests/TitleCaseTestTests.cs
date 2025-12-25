using TagSelecta.Cli.Commands.TagDataCommands;
using TagSelecta.Cli.Commands.TagDataCommands.Common;
using TagSelecta.Tagging;

namespace TagSelecta.Cli.Tests;

[Collection("Console")]
public class TitleCaseTests
{
    [Fact]
    public async Task TitleCaseTest()
    {
        // Arrange
        var action = new TitleCaseAction();

        var settings = new TitleCaseSettings();

        var tagData = new TagData() { Title = "test title", Artist = ["test artist"] };

        var context = new TagDataActionContext<TitleCaseSettings>
        {
            Files = ["file1.mp3"],
            Settings = settings,
        };

        // Act
        context.SetCurrentFile(context.Files[0], 0, tagData);
        await action.ProcessTagDataAsync(context);

        // Assert
        Assert.Equal("Test Title", tagData.Title);
        Assert.Equal("Test Artist", tagData.Artist[0]);
    }
}
