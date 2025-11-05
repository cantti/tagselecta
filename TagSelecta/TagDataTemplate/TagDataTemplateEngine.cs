using Scriban;
using TagSelecta.Tagging;

namespace TagSelecta.TagDataTemplate;

public static class TagDataTemplateEngine
{
    public static string Format(string format, TagData tagData, string path)
    {
        var model = TagDataTemplateModelMapper.Map(tagData);
        model.Path = path;
        model.Filename = Path.GetFileNameWithoutExtension(path);
        var template = Template.Parse(format);
        var result = template.Render(model, member => member.Name.ToLower());
        return result;
    }
}
