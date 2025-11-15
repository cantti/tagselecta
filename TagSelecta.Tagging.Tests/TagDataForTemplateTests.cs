using System.Reflection;

namespace TagSelecta.Tagging.Tests;

public class TagDataForTemplateTests
{
    [Fact]
    public void All_TagDataField_Properties_Are_Mapped()
    {
        var tagDataProps = typeof(TagData)
            .GetProperties()
            .Where(p => p.GetCustomAttribute<TagDataFieldAttribute>() != null)
            .Select(p => p.Name)
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
