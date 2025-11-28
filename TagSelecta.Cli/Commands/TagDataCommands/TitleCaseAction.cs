using System.Globalization;
using System.Reflection;
using Spectre.Console;
using TagSelecta.Tagging;

namespace TagSelecta.Cli.Commands.TagDataCommands;

public class TitleCaseSettings : BaseSettings { }

public class TitleCaseAction : TagDataAction<TitleCaseSettings>
{
    protected override void ProcessTagData(TagDataActionContext<TitleCaseSettings> context)
    {
        foreach (
            var prop in typeof(TagData)
                .GetProperties()
                .Where(x => x.GetCustomAttribute<BuiltinFieldAttribute>() is not null)
        )
        {
            var value =
                prop.GetValue(context.TagData)
                ?? throw new InvalidOperationException("TagData values can not be null!");
            if (prop.PropertyType == typeof(string))
            {
                var str = (string)value;
                prop.SetValue(context.TagData, ToTitleCase(str));
            }
            if (prop.PropertyType == typeof(List<string>))
            {
                var list = (List<string>)value;
                prop.SetValue(context.TagData, list.Select(ToTitleCase).ToList());
            }
        }
    }

    private static string ToTitleCase(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;
        return textInfo.ToTitleCase(input.ToLowerInvariant());
    }
}
