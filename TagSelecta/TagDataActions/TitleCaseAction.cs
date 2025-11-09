using System.Globalization;
using System.Reflection;
using Spectre.Console;
using TagSelecta.Tagging;

namespace TagSelecta.TagDataActions;

public class TitleCaseSettings : BaseSettings { }

public class TitleCaseAction : ITagDataAction<TitleCaseSettings>
{
    public Task ProcessTagData(TagDataActionContext<TitleCaseSettings> context)
    {
        foreach (
            var prop in typeof(TagData)
                .GetProperties()
                .Where(x => x.GetCustomAttribute<EditableAttribute>() is not null)
        )
        {
            if (prop.GetValue(context.TagData) is string value)
            {
                prop.SetValue(context.TagData, ToTitleCase(value));
            }
            if (prop.GetValue(context.TagData) is List<string> valueList)
            {
                prop.SetValue(context.TagData, valueList.Select(ToTitleCase).ToList());
            }
        }
        return Task.CompletedTask;
    }

    private static string ToTitleCase(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;
        return textInfo.ToTitleCase(input.ToLowerInvariant());
    }
}
