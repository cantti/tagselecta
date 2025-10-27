using SmartFormat;
using SmartFormat.Core.Settings;

namespace SongTagCli.Misc;

public static class TagTemplateFormatter
{
    private static readonly SmartFormatter _formatter = Smart.CreateDefaultSmartFormat(
        new SmartSettings { CaseSensitivity = CaseSensitivityType.CaseInsensitive }
    );
    public static SmartFormatter Formatter => _formatter;

    public static string Format(string format, TagTemplateContext context)
    {
        return _formatter.Format(format, context);
    }
}
