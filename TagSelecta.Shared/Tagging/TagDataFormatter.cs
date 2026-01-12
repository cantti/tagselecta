using Scriban;

namespace TagSelecta.Shared.Tagging;

public class TagDataFormatter
{
    private readonly TagDataForTemplate _tagDataForTemplate;

    public TagDataFormatter(TagData tagData, string path)
    {
        _tagDataForTemplate = new TagDataForTemplate(tagData, path);
    }

    public string Format(string template)
    {
        var parsedTemplate = Template.Parse(template);
        var result = parsedTemplate.Render(_tagDataForTemplate, member => member.Name.ToLower());
        return result.Trim();
    }
}
