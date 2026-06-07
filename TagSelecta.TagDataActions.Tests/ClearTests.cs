using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;
using TagSelecta.TagDataActions.Clear;
using TagSelecta.TagDataActions.Tests.Utils;

namespace TagSelecta.TagDataActions.Tests;

public class ClearTests
{
    [Fact]
    public async Task ClearTest()
    {
        // Arrange
        ITagDataAction action = new ClearAction();

        var settings = new ClearSettings { Album = true, Key = ["title"] };
        settings.Remaining.Add(new RemainingArgument("publisher", ""));

        var tagData = new TagData();
        tagData.SetValue(FieldName.Album, "Album");
        tagData.SetValue(FieldName.Artist, "Artist");
        tagData.SetValue(FieldName.Title, "Title");
        tagData.SetValue("label", "Label");
        tagData.SetValue("publisher", "Publisher");

        var item = new TestTarget("file.mp3", tagData);

        await action.BeforeExecute(settings, CancellationToken.None);

        // Act
        await action.Execute(
            new TagDataActionExecuteContext { Settings = settings, Target = item },
            CancellationToken.None
        );

        // Assert
        var currentTagData = item.CurrentTagData;
        Assert.Equal([], currentTagData.GetValue(FieldName.Album));
        Assert.Equal([], currentTagData.GetValue(FieldName.Title));
        Assert.Equal([], currentTagData.GetValue("publisher"));
        Assert.Equal(["Artist"], currentTagData.GetValue(FieldName.Artist));
        Assert.Equal(["Label"], currentTagData.GetValue("label"));
    }

    [Fact]
    public void ClearSettingsContainsAllFieldNames()
    {
        var properties = typeof(ClearSettings).GetProperties();
        var propertyNames = new HashSet<string>(
            properties.Select(x => x.Name),
            StringComparer.OrdinalIgnoreCase
        );

        var missing = FieldName.All().Where(field => !propertyNames.Contains(field)).ToList();

        Assert.True(
            missing.Count == 0,
            $"ClearSettings is missing fields: {string.Join(", ", missing)}"
        );
    }
}
