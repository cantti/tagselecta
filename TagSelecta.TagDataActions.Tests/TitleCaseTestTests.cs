using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;
using TagSelecta.TagDataActions.Tests.Utils;
using TagSelecta.TagDataActions.TitleCase;

namespace TagSelecta.TagDataActions.Tests;

public class TitleCaseTests
{
    [Fact]
    public async Task TitleCaseTest()
    {
        // Arrange
        ITagDataAction action = new TitleCaseAction();

        var settings = new TitleCaseSettings();

        var tagData = new TagData();
        tagData.SetValue(FieldName.Title, "test title");
        tagData.SetValue(FieldName.Artist, ["test artist"]);

        var item = new TestTarget("file.mp3", tagData);

        // Act
        await action.Execute(
            new TagDataActionExecuteContext { Settings = settings, Target = item },
            CancellationToken.None
        );

        // Assert
        var newTagData = item.CurrentTagData;
        Assert.Equal(["Test Title"], newTagData.GetValue(FieldName.Title));
        Assert.Equal(["Test Artist"], newTagData.GetValue(FieldName.Artist));
    }
}
