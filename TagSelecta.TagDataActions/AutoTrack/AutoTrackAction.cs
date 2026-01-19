using TagSelecta.Shared.TagDataActions;

namespace TagSelecta.TagDataActions.AutoTrack;

[TagDataActionName("autotrack")]
public class AutoTrackAction : TagDataAction<AutoTrackSettings>
{
    protected override void Execute(TagDataActionExecuteContext<AutoTrackSettings> context)
    {
        var tagData = context.Target.CurrentTagData;
        tagData.Track = (
            context.DirectoryFiles.ToList().FindIndex(x => x.Path == context.Target.BackupPath) + 1
        ).ToString();
        tagData.TrackTotal = context.DirectoryFiles.Count().ToString();
        if (!context.Settings.KeepDisk)
        {
            tagData.Disc = "";
            tagData.DiscTotal = "";
        }
        context.Target.UpdateTagData(tagData);
    }
}
