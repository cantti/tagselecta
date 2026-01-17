using TagSelecta.Shared.TagDataActions;

namespace TagSelecta.TagDataActions.AutoTrack;

[TagDataActionName("autotrack")]
public class AutoTrackAction : TagDataAction<AutoTrackSettings>
{
    protected override void Execute(
        ITagDataActionContext current,
        IEnumerable<ITagDataActionContext> files,
        AutoTrackSettings settings
    )
    {
        var dir = Path.GetDirectoryName(current.BackupPath);
        var filesInDir = files
            .Where(x => Path.GetDirectoryName(x.BackupPath) == dir)
            .OrderBy(x => x.BackupPath)
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
