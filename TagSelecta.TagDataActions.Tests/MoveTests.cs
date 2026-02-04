using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;
using TagSelecta.TagDataActions.Move;
using TagSelecta.TagDataActions.Tests.Utils;

namespace TagSelecta.TagDataActions.Tests;

public class MoveTests
{
    [Fact]
    public async Task MoveTest()
    {
        // Arrange
        ITagDataAction action = new MoveAction();

        var settings = new MoveSettings { Template = "{{ year }} - {{album}}/{{filename}}" };

        var tagData = new TagData { Date = "1990", Album = "Test Album" };

        var item = new TestTarget("/file.mp3", tagData);

        // Act
        await action.Execute(
            new TagDataActionExecuteContext { Target = item, Settings = settings },
            CancellationToken.None
        );

        // Assert
        Assert.Equal("/1990 - Test Album/file.mp3", item.CurrentPath);
    }

    [Fact]
    public async Task MoveTest_Relative()
    {
        // Arrange
        ITagDataAction action = new MoveAction();

        var settings = new MoveSettings { Template = "../{{ year }} - {{album}}/{{filename}}" };

        var tagData = new TagData { Date = "1990", Album = "Test Album" };

        var item = new TestTarget("/dir/file.mp3", tagData);

        // Act
        await action.Execute(
            new TagDataActionExecuteContext { Target = item, Settings = settings },
            CancellationToken.None
        );

        // Assert
        Assert.Equal("/1990 - Test Album/file.mp3", item.CurrentPath);
    }
}
