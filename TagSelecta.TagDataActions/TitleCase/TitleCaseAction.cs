using System.Globalization;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.TagDataActions.TitleCase;

[TagDataActionInfo("titlecase")]
public class TitleCaseAction : ITagDataAction<TitleCaseSettings>
{
    public Task<bool> BeforeExecute(TitleCaseSettings settings, CancellationToken token)
    {
        return Task.FromResult(true);
    }

    public Task Execute(TagDataActionExecuteContext<TitleCaseSettings> context, CancellationToken token)
    {
        var tagData = context.Target.CurrentTagData;
        foreach (var field in tagData.Fields)
        {
            tagData.SetValue(field.Key, field.Text.Select(ToTitleCase).ToList());
        }

        context.Target.UpdateTagData(tagData);

        return Task.CompletedTask;
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
