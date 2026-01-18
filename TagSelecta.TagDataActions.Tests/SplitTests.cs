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
        await action.ExecuteAsync(TODO, CancellationToken.None);

        // Assert
        var newTagData = item.GetCurrentTagData();
        Assert.Equal("Artist1", newTagData.Artist[0]);
        Assert.Equal("Artist2", newTagData.Artist[1]);
    }
}
