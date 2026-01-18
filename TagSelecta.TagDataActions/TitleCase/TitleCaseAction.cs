using System.Globalization;
using TagSelecta.Shared.TagDataActions;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.TagDataActions.TitleCase;

[TagDataActionName("titlecase")]
public class TitleCaseAction : TagDataAction<TitleCaseSettings>
{
    protected override void Execute(TagDataActionExecuteContext<TitleCaseSettings> context)
    {
        var tagData = context.Target.GetCurrentTagData();
        foreach (
            var prop in typeof(TagData)
                .GetProperties()
                .Where(p => p.Name != nameof(TagData.Picture))
                .Where(p => p.Name != nameof(TagData.Custom))
        )
        {
            var value = prop.GetValue(tagData)!;
            if (prop.PropertyType == typeof(string))
            {
                var str = (string)value;
                prop.SetValue(tagData, ToTitleCase(str));
            }
            if (prop.PropertyType == typeof(List<string>))
            {
                var list = (List<string>)value;
                prop.SetValue(tagData, list.Select(ToTitleCase).ToList());
            }
        }
        context.Target.SetCurrentTagData(tagData);
    }

    private static string ToTitleCase(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;
        return textInfo.ToTitleCase(input.ToLowerInvariant());
    }
}
