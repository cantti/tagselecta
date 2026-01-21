using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;
using TagSelecta.TagDataActions.Split;
using TagSelecta.TagDataActions.Tests.Utils;

namespace TagSelecta.TagDataActions.Tests;

public class SplitTests
{
    [Fact]
    public async Task SplitTest()
    {
        // Arrange
        ITagDataAction action = new SplitAction();
        var settings = new SplitSettings();
        var tagData = new TagData() { Artist = ["Artist1; Artist2"] };
        var item = new TestTarget("file.mp3", tagData);

        // Act
        await action.Execute(
            new TagDataActionExecuteContext { Settings = settings, Target = item },
            CancellationToken.None
        );

        // Assert
        var newTagData = item.CurrentTagData;
        Assert.Equal("Artist1", newTagData.Artist[0]);
        Assert.Equal("Artist2", newTagData.Artist[1]);
    }
}
