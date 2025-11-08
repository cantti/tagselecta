using System.Globalization;
using System.Reflection;
using Spectre.Console;
using TagSelecta.BaseCommands;
using TagSelecta.Tagging;

namespace TagSelecta.Commands;

public class TitleCaseSettings : BaseSettings { }

public class TitleCaseCommand(IAnsiConsole console) : TagDataProcessingCommand<TitleCaseSettings>(console)
{
    protected override void ProcessTagData()
    {
        foreach (
            var prop in typeof(TagData)
                .GetProperties()
                .Where(x => x.GetCustomAttribute<EditableAttribute>() is not null)
        )
        {
            if (prop.GetValue(TagData) is string value)
            {
                prop.SetValue(TagData, ToTitleCase(value));
            }
            if (prop.GetValue(TagData) is List<string> valueList)
            {
                prop.SetValue(TagData, valueList.Select(ToTitleCase).ToList());
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
