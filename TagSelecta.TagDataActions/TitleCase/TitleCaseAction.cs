using System.Globalization;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.TagDataActions.TitleCase;

[TagDataActionInfo("titlecase")]
public class TitleCaseAction : TagDataAction<TitleCaseSettings>
{
    protected override void Execute(TagDataActionExecuteContext<TitleCaseSettings> context)
    {
        var tagData = context.Target.CurrentTagData;
        foreach (var field in tagData.Fields)
        {
            tagData.SetValue(field.Key, field.Text.Select(ToTitleCase).ToList());
        }
        context.Target.UpdateTagData(tagData);
    }

    private static string ToTitleCase(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        var textInfo = CultureInfo.InvariantCulture.TextInfo;
        return textInfo.ToTitleCase(input.ToLowerInvariant());
    }
}
