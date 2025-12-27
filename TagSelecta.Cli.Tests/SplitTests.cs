using TagSelecta.Cli.Commands.Common;
using TagSelecta.Cli.Commands.Split;
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
        var item = new FileWithTagData { Path = "file.mp3", TagData = tagData };

        // Act
        await action.ProcessTagDataAsync(item, [item], settings, StringLookup.Empty());

        // Assert
        Assert.Equal("Artist1", tagData.Artist[0]);
        Assert.Equal("Artist2", tagData.Artist[1]);
    }
}
