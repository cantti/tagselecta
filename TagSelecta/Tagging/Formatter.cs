using Scriban;

namespace TagSelecta.Tagging;

public static class Formatter
{
    public static string Format(string template, TagData tagData)
    {
        var parsedTemplate = Template.Parse(template);
        var result = parsedTemplate.Render(tagData, member => member.Name.ToLower());
        return result;
    }
}
