using Scriban;
using TagSelecta.Tagging;

namespace TagSelecta.Formatting;

public static class Formatter
{
    public static string Format(string template, TagData tagData)
    {
        var parsedTemplate = Template.Parse(template);
        var result = parsedTemplate.Render(tagData, member => member.Name.ToLower());
        return result;
    }
}
