using TagSelecta.Shared.TagDataActions;
using TagSelecta.TagDataActions.TitleCase;
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

        var item = new TagDataOperation("file.mp3", tagData);

        // Act
        await action.ExecuteAsync(item, [item], settings, CancellationToken.None);

        // Assert
        Assert.Equal("Test Title", tagData.Title);
        Assert.Equal("Test Artist", tagData.Artist[0]);
    }
}
