using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.TagDataActions.Split;

[TagDataActionInfo("split")]
public class SplitAction : TagDataAction<SplitSettings>
{
    protected override void Execute(TagDataActionExecuteContext<SplitSettings> context)
    {
        var tagData = context.Target.CurrentTagData;
        var separator = context.Settings.Separator;

        string[] fields =
        [
            FieldName.Artist,
            FieldName.AlbumArtist,
            FieldName.Composer,
            FieldName.Genre,
        ];

        foreach (var field in fields)
        {
            tagData.SetValue(field, Split(tagData.GetValue(field), separator));
        }

        context.Target.UpdateTagData(tagData);
    }

    private static List<string> Split(List<string> input, string[] settingsSeparator)
    {
        return input
            .SelectMany(x => x.Split(settingsSeparator, StringSplitOptions.RemoveEmptyEntries))
            .Select(x => x.Trim())
            .ToList();
    }
}
