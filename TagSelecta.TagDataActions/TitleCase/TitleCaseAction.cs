using System.Globalization;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.TagDataActions.TitleCase;

[TagDataActionName("titlecase")]
public class TitleCaseAction : TagDataAction<TitleCaseSettings>
{
    protected override void Execute(TagDataActionExecuteContext<TitleCaseSettings> context)
    {
        var tagData = context.Target.CurrentTagData;
        foreach (
            var prop in typeof(TagData)
                .GetProperties()
                .Where(p => p.Name != nameof(TagData.Picture))
                .Where(p => p.Name != nameof(TagData.Extra))
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
        context.Target.UpdateTagData(tagData);
    }

    private static string ToTitleCase(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;
        return textInfo.ToTitleCase(input.ToLowerInvariant());
    }
}
