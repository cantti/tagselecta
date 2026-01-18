using TagSelecta.Shared.TagDataActions;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Move;

namespace TagSelecta.TagDataActions.Tests;

public class MoveTests
{
    [Fact]
    public async Task MoveTest()
    {
        // Arrange
        var action = new MoveAction();

        var settings = new MoveSettings { Template = "{{ year }} - {{album}}/{{filename}}" };

        var tagData = new TagData() { Date = "1990", Album = "Test Album" };

        var item = new TagDataOperation("/file.mp3", tagData);

        // Act
        await action.ExecuteAsync(
            new TagDataActionExecuteContext<MoveSettings>
            {
                Files = [item],
                Target = item,
                Settings = settings,
            },
            CancellationToken.None
        );

        // Assert
        Assert.Equal("/1990 - Test Album/file.mp3", item.GetCurrentPath());
    }

    [Fact]
    public async Task MoveTest_Relative()
    {
        // Arrange
        var action = new MoveAction();

        var settings = new MoveSettings { Template = "../{{ year }} - {{album}}/{{filename}}" };

        var tagData = new TagData() { Date = "1990", Album = "Test Album" };

        var item = new TagDataOperation("/dir/file.mp3", tagData);

        // Act
        await action.ExecuteAsync(
            new TagDataActionExecuteContext<MoveSettings>
            {
                Files = [item],
                Target = item,
                Settings = settings,
            },
            CancellationToken.None
        );

        // Assert
        Assert.Equal("/1990 - Test Album/file.mp3", item.GetCurrentPath());
    }
}
