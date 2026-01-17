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

    protected override void Execute(
        ITagDataActionContext current,
        IEnumerable<ITagDataActionContext> files,
        SplitSettings settings
    )
    {
        var artists = current.CurrentTagData.Artist.SelectMany(Split).Distinct().ToList();
        var albumArtists = current.CurrentTagData.AlbumArtist.SelectMany(Split).Distinct().ToList();
        var composers = current
            .CurrentTagData.Composer.Select(Split)
            .SelectMany(x => x)
            .Distinct()
            .ToList();

        current.CurrentTagData.Artist = artists;
        current.CurrentTagData.AlbumArtist = albumArtists;
        current.CurrentTagData.Composer = composers;
    }

    private List<string> Split(string input)
    {
        return input
            .Split(_separators, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .ToList();
    }
}
