using Scriban;
using TagSelecta.Tagging;

namespace TagSelecta.Formatting;

public static class Formatter
{
    public static string Format(string template, TagData tagData, string path)
    {
        var model = FormatterModelMapper.Map(tagData);
        model.Path = path;
        model.Filename = Path.GetFileNameWithoutExtension(path);
        var parsedTemplate = Template.Parse(template);
        var result = parsedTemplate.Render(model, member => member.Name.ToLower());
        return result;
    }
}
