using TagSelecta.Shared.TagDataActions;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Split;

namespace TagSelecta.TagDataActions.Tests;

public class SplitTests
{
    [Fact]
    public async Task SplitTest()
    {
        // Arrange
        var action = new SplitAction();
        var settings = new SplitSettings();
        var tagData = new TagData() { Artist = ["Artist1; Artist2"] };
        var item = new TagDataOperation("file.mp3", tagData);

        // Act
        await action.ExecuteAsync(item, [item], settings, CancellationToken.None);

        // Assert
        Assert.Equal("Artist1", tagData.Artist[0]);
        Assert.Equal("Artist2", tagData.Artist[1]);
    }
}
