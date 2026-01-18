using TagSelecta.Shared.TagDataActions;

namespace TagSelecta.TagDataActions.AutoTrack;

[TagDataActionName("autotrack")]
public class AutoTrackAction : TagDataAction<AutoTrackSettings>
{
    protected override void Execute(TagDataActionExecuteContext<AutoTrackSettings> context)
    {
        var tagData = context.Target.GetCurrentTagData();
        var dir = Path.GetDirectoryName(context.Target.GetBackupPath());
        var filesInDir = context
            .Files.Where(x => Path.GetDirectoryName(x.GetBackupPath()) == dir)
            .OrderBy(x => x.GetBackupPath())
            .ToList();
        tagData.Track = (
            filesInDir.FindIndex(x => x.GetBackupPath() == context.Target.GetBackupPath()) + 1
        ).ToString();
        tagData.TrackTotal = filesInDir.Count.ToString();
        if (!context.Settings.KeepDisk)
        {
            tagData.Disc = "";
            tagData.DiscTotal = "";
        }
        context.Target.SetCurrentTagData(tagData);
    }
}
