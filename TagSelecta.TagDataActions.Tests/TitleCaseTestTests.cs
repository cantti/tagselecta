using TagSelecta.Shared.TagDataActions;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.TitleCase;

namespace TagSelecta.TagDataActions.Tests;

public class TitleCaseTests
{
    [Fact]
    public async Task TitleCaseTest()
    {
        // Arrange
        var action = new TitleCaseAction();

        var settings = new TitleCaseSettings();

        var tagData = new TagData() { Title = "test title", Artist = ["test artist"] };

        var item = new TagDataActionTarget("file.mp3", tagData);

        // Act
        await action.ExecuteAsync(
            new TagDataActionExecuteContext<TitleCaseSettings>
            {
                DirectoryFiles = [new TagDataActionFileInfo(item.BackupPath)],
                Settings = settings,
                Target = item,
            },
            CancellationToken.None
        );

        // Assert
        var newTagData = item.CurrentTagData;
        Assert.Equal("Test Title", newTagData.Title);
        Assert.Equal("Test Artist", newTagData.Artist[0]);
    }
}
