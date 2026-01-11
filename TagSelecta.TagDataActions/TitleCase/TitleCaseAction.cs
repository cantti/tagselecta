using System.Globalization;
using TagSelecta.Shared.TagDataActions;
using TagSelecta.Tagging;

namespace TagSelecta.TagDataActions.TitleCase;

[TagDataActionName("titlecase")]
public class TitleCaseAction : TagDataAction<TitleCaseSettings>
{
    protected override void ProcessTagData(
        IFileContext current,
        IEnumerable<IFileContext> files,
        TitleCaseSettings settings
    )
    {
        foreach (
            var prop in typeof(TagData)
                .GetProperties()
                .Where(p => p.Name != nameof(TagData.Picture))
                .Where(p => p.Name != nameof(TagData.Custom))
        )
        {
            var value = prop.GetValue(current.CurrentTagData)!;
            if (prop.PropertyType == typeof(string))
            {
                var str = (string)value;
                prop.SetValue(current.CurrentTagData, ToTitleCase(str));
            }
            if (prop.PropertyType == typeof(List<string>))
            {
                var list = (List<string>)value;
                prop.SetValue(current.CurrentTagData, list.Select(ToTitleCase).ToList());
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
