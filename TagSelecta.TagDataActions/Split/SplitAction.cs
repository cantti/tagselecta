using TagSelecta.Shared.TagDataActions;

namespace TagSelecta.TagDataActions.Split;

[TagDataActionName("split")]
public class SplitAction : TagDataAction<SplitSettings>
{
    private string[] _separators = [",", ";", "feat."];

    protected override bool BeforeExecute(SplitSettings settings)
    {
        if (settings.Separator is not null)
        {
            _separators = settings.Separator;
        }
        return true;
    }

    protected override void Execute(TagDataActionExecuteContext<SplitSettings> context)
    {
        var tagData = context.Target.CurrentTagData;
        var artists = tagData.Artist.SelectMany(Split).Distinct().ToList();
        var albumArtists = tagData.AlbumArtist.SelectMany(Split).Distinct().ToList();
        var composers = tagData.Composer.Select(Split).SelectMany(x => x).Distinct().ToList();

        tagData.Artist = artists;
        tagData.AlbumArtist = albumArtists;
        tagData.Composer = composers;
        context.Target.UpdateTagData(tagData);
    }

    private List<string> Split(string input)
    {
        return input
            .Split(_separators, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .ToList();
    }
}
