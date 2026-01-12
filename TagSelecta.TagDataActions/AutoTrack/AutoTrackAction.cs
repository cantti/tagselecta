using TagSelecta.Shared.TagDataActions;

namespace TagSelecta.TagDataActions.AutoTrack;

[TagDataActionName("autotrack")]
public class AutoTrackAction : TagDataAction<AutoTrackSettings>
{
    protected override void Execute(
        IFileContext current,
        IEnumerable<IFileContext> files,
        AutoTrackSettings settings
    )
    {
        var dir = Path.GetDirectoryName(current.OriginalPath);
        var filesInDir = files
            .Where(x => Path.GetDirectoryName(x.OriginalPath) == dir)
            .OrderBy(x => x.OriginalPath)
            .ToList();
        current.CurrentTagData.Track = (filesInDir.IndexOf(current) + 1).ToString();
        current.CurrentTagData.TrackTotal = filesInDir.Count.ToString();
        if (!settings.KeepDisk)
        {
            current.CurrentTagData.Disc = "";
            current.CurrentTagData.DiscTotal = "";
        }
    }
}
