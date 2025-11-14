using Scriban;

namespace TagSelecta.Tagging;

public class TagDataFormatter(TagData tagData, string path)
{
    public string Format(string template)
    {
        var parsedTemplate = Template.Parse(template);
        var result = parsedTemplate.Render(
            new TagDataForTemplate(tagData, path),
            member => member.Name.ToLower()
        );
        return result;
    }
}
