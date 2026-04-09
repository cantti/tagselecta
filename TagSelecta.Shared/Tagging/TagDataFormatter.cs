using System.Globalization;
using System.Text;
using Scriban;
using Scriban.Runtime;
using TagSelecta.Shared.Exceptions;

namespace TagSelecta.Shared.Tagging;

public class TagDataFormatter
{
    private readonly bool _sanitize;
    private readonly Dictionary<string, string> _tagDataForTemplate;

    public TagDataFormatter(TagData tagData, string path, bool sanitize = false)
    {
        _sanitize = sanitize;
        _tagDataForTemplate = new Dictionary<string, string>();
        foreach (var field in tagData.Fields)
        {
            _tagDataForTemplate.Add(field.Key, S(field.Text.JoinTagValues()));
        }
        _tagDataForTemplate.Add("path", S(path));
        _tagDataForTemplate.Add("filename", S(Path.GetFileNameWithoutExtension(path)));
        _tagDataForTemplate.Add("ext", S(Path.GetExtension(path).TrimStart('.')));
        _tagDataForTemplate.Add(
            "year",
            DateTime.TryParseExact(
                tagData.GetValueFirst(FieldName.Date),
                ["yyyy", "yyyy-MM-dd", "yyyy/MM/dd"],
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var d
            )
                ? d.Year.ToString()
                : ""
        );
    }

    private string S(string input)
    {
        if (!_sanitize)
        {
            return input;
        }

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

    public string Format(string template)
    {
        MemberRenamerDelegate memberRenamer = member => member.Name.ToLowerInvariant();
        var scriptObject = new ScriptObject();
        scriptObject.Import(typeof(Functions), renamer: memberRenamer);
        scriptObject.Import(_tagDataForTemplate, renamer: memberRenamer);
        var context = new TemplateContext();
        context.PushGlobal(scriptObject);
        var parsedTemplate = Template.Parse(template);
        if (parsedTemplate.HasErrors)
        {
            throw new TagSelectaException(parsedTemplate.Messages.ToString());
        }

        var result = parsedTemplate.Render(context);
        return result.Trim();
    }
}

public static class Functions
{
    public static string Pad(string? input, int totalWidth = 2)
    {
        input ??= string.Empty;
        return input.PadLeft(totalWidth, '0');
    }
}
