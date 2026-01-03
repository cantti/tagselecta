using TagSelecta.Cli.Commands.TagDataCommandShared;

namespace TagSelecta.Cli.Commands.Split;

public class SplitAction : TagDataAction<SplitSettings>
{
    private string[] _separators = [",", ";", "feat."];

    protected override bool BeforeProcessTagData(SplitSettings settings)
    {
        if (settings.Separator is not null)
        {
            _separators = settings.Separator;
        }
        return true;
    }

    protected override void ProcessTagData(
        FileWithTagData current,
        List<FileWithTagData> files,
        SplitSettings settings
    )
    {
        var artists = current.TagData.Artist.SelectMany(Split).Distinct().ToList();
        var albumArtists = current.TagData.AlbumArtist.SelectMany(Split).Distinct().ToList();
        var composers = current
            .TagData.Composer.Select(Split)
            .SelectMany(x => x)
            .Distinct()
            .ToList();

        current.TagData.Artist = artists;
        current.TagData.AlbumArtist = albumArtists;
        current.TagData.Composer = composers;
    }

    private List<string> Split(string input)
    {
        return input
            .Split(_separators, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .ToList();
    }
}
