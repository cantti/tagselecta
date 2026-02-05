using System.Text;
using Scriban;
using Scriban.Runtime;

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
        var scriptObject = new ScriptObject();
        scriptObject.Import(typeof(MyFunctions));
        scriptObject.Import(_tagDataForTemplate);
        var context = new TemplateContext();
        context.PushGlobal(scriptObject);
        context.MemberRenamer = member => member.Name.ToLower();
        var parsedTemplate = Template.Parse(template);
        var result = parsedTemplate.Render(context);
        return result.Trim();
    }
}

public static class MyFunctions
{
    public static string Escape(string input)
    {
        var s = input.Replace('/', '_').Replace('\\', '_');
        var invalid = Path.GetInvalidFileNameChars();
        var sb = new StringBuilder(s.Length);
        foreach (var ch in s)
        {
            sb.Append(invalid.Contains(ch) ? '_' : ch);
        }

        s = sb.ToString();

        // collapse repeated underscores
        while (s.Contains("__", StringComparison.Ordinal))
        {
            s = s.Replace("__", "_", StringComparison.Ordinal);
        }

        return s;
    }
}
