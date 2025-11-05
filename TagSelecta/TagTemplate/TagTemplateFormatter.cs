using Scriban;
using TagSelecta.Tagging;

namespace TagSelecta.TagTemplate;

public static class TagTemplateFormatter
{
    public static string Format(string format, TagData tagData)
    {
        var template = Template.Parse(format);
        var result = template.Render(
            TagTemplateContextMapper.Map(tagData),
            member => member.Name.ToLower()
        );
        return result;
    }
}
