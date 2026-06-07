using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;
using TagSelecta.TagDataActions.ClearExcept;
using TagSelecta.TagDataActions.Tests.Utils;

namespace TagSelecta.TagDataActions.Tests;

public class ClearExceptTests
{
    [Fact]
    public async Task ClearExceptTest()
    {
        // Arrange
        ITagDataAction action = new ClearExceptAction();

        var settings = new ClearExceptSettings { Album = true, Key = ["title"] };

        var tagData = new TagData();
        tagData.SetValue(FieldName.Album, "Album");
        tagData.SetValue(FieldName.Artist, "Artist");
        tagData.SetValue(FieldName.Title, "Title");
        tagData.SetValue("label", "Label");

        var item = new TestTarget("file.mp3", tagData);

        await action.BeforeExecute(settings, CancellationToken.None);

        // Act
        await action.Execute(
            new TagDataActionExecuteContext { Settings = settings, Target = item },
            CancellationToken.None
        );

        // Assert
        var currentTagData = item.CurrentTagData;
        Assert.Equal(["Album"], currentTagData.GetValue(FieldName.Album));
        Assert.Equal(["Title"], currentTagData.GetValue(FieldName.Title));
        Assert.Equal([], currentTagData.GetValue(FieldName.Artist));
        Assert.Equal([], currentTagData.GetValue("label"));
    }

    [Fact]
    public void ClearExceptSettingsContainsAllFieldNames()
    {
        var properties = typeof(ClearExceptSettings).GetProperties();
        var propertyNames = new HashSet<string>(
            properties.Select(x => x.Name),
            StringComparer.OrdinalIgnoreCase
        );

        var missing = FieldName.All().Where(field => !propertyNames.Contains(field)).ToList();

        Assert.True(
            missing.Count == 0,
            $"ClearExceptSettings is missing fields: {string.Join(", ", missing)}"
        );
    }
}
