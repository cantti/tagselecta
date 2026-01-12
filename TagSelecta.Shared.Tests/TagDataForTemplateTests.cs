using TagSelecta.Shared.Tagging;

namespace TagSelecta.Shared.Tests;

public class TagDataForTemplateTests
{
    [Fact]
    public void All_TagDataField_Properties_Are_Mapped()
    {
        var tagDataProps = typeof(TagData)
            .GetProperties()
            .Select(p => p.Name)
            .Where(p => p != nameof(TagData.Picture))
            .Where(p => p != nameof(TagData.Custom))
            .ToHashSet();

        var templateProps = typeof(TagDataForTemplate)
            .GetProperties()
            .Select(p => p.Name)
            .ToHashSet();

        foreach (var p in tagDataProps)
        {
            Assert.Contains(p, templateProps);
        }
    }
}
