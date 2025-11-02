using SmartFormat;
using SmartFormat.Core.Settings;
using TagSelecta.Tagging;

namespace TagSelecta.TagTemplate;

public static class TagTemplateFormatter
{
    private static readonly SmartFormatter _formatter = Smart.CreateDefaultSmartFormat(
        new SmartSettings { CaseSensitivity = CaseSensitivityType.CaseInsensitive }
    );
    public static SmartFormatter Formatter => _formatter;

    public static string Format(string format, TagData tagData)
    {
        return _formatter.Format(format, TagTemplateContextMapper.Map(tagData));
    }
}
