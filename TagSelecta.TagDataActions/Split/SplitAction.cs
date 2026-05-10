using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.TagDataActions.Split;

[TagDataActionInfo("split")]
public class SplitAction : ITagDataAction<SplitSettings>
{
    public Task<bool> BeforeExecute(SplitSettings settings, CancellationToken token)
    {
        return Task.FromResult(true);
    }

    public Task Execute(TagDataActionExecuteContext<SplitSettings> context, CancellationToken token)
    {
        var tagData = context.Target.CurrentTagData;
        var separator = context.Settings.Separator;

        // todo make this configurable
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

        return Task.CompletedTask;
    }

    private static List<string> Split(List<string> input, string[] settingsSeparator)
    {
        return input
            .SelectMany(x => x.Split(settingsSeparator, StringSplitOptions.RemoveEmptyEntries))
            .Select(x => x.Trim())
            .ToList();
    }
}
