using System.Globalization;
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
                .Where(p => p.Name != nameof(TagData.Picture))
                .Where(p => p.Name != nameof(TagData.Custom))
        )
        {
            var value = prop.GetValue(context.TagData)!;
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
