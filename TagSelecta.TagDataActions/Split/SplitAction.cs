using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.TagDataActions.Split;

[TagDataActionName("split")]
public class SplitAction : TagDataAction<SplitSettings>
{
    protected override void Execute(TagDataActionExecuteContext<SplitSettings> context)
    {
        var tagData = context.Target.CurrentTagData;
        var separator = context.Settings.Separator;
        tagData.Artist = Split(tagData.Artist, separator);
        tagData.AlbumArtist = Split(tagData.AlbumArtist, separator);
        tagData.Composer = Split(tagData.Composer, separator);
        tagData.Genre = Split(tagData.Genre, separator);
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
