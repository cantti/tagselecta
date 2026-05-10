using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.ClearExcept;

namespace TagSelecta.TagDataActions.Tests;

public class ClearExceptTests
{
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
