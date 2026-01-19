using TagSelecta.Shared.IO;
using TagSelecta.Shared.TagDataActions;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.TagDataActions.AutoTrack;

[TagDataActionName("autotrack")]
public class AutoTrackAction(IAudioFileScanner fileScanner) : TagDataAction<AutoTrackSettings>
{
    protected override void Execute(TagDataActionExecuteContext<AutoTrackSettings> context)
    {
        var tagData = context.Target.CurrentTagData;
        var directoryFiles = fileScanner
            .Search([context.Target.BackupPath.DirectoryName()])
            .Order()
            .ToList();
        tagData.Track = (
            directoryFiles.ToList().FindIndex(x => x == context.Target.BackupPath) + 1
        ).ToString();
        tagData.TrackTotal = directoryFiles.Count.ToString();
        if (!context.Settings.KeepDisk)
        {
            tagData.Disc = "";
            tagData.DiscTotal = "";
        }
        context.Target.UpdateTagData(tagData);
    }
}
