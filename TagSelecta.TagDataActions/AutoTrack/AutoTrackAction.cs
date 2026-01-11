using TagSelecta.Shared.TagDataActions;

namespace TagSelecta.TagDataActions.AutoTrack;

[TagDataActionName("autotrack")]
public class AutoTrackAction : TagDataAction<AutoTrackSettings>
{
    protected override void ProcessTagData(
        IFileContext current,
        IEnumerable<IFileContext> files,
        AutoTrackSettings settings
    )
    {
        var dir = Directory.GetParent(current.CurrentPath)?.FullName;
        var filesInDir = files
            .Select(x => x.CurrentPath)
            .Where(x => Directory.GetParent(x)?.FullName == dir)
            .Order()
            .ToList();
        current.CurrentTagData.Track = (filesInDir.IndexOf(current.CurrentPath) + 1).ToString();
        current.CurrentTagData.TrackTotal = filesInDir.Count.ToString();
        if (!settings.KeepDisk)
        {
            current.CurrentTagData.Disc = "";
            current.CurrentTagData.DiscTotal = "";
        }
    }
}
