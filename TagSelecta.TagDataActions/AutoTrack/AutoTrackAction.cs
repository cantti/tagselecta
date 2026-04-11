using TagSelecta.Shared.IO;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.TagDataActions.AutoTrack;

[TagDataActionInfo("autotrack")]
public class AutoTrackAction(IAudioFileScanner fileScanner) : TagDataAction<AutoTrackSettings>
{
    protected override void Execute(TagDataActionExecuteContext<AutoTrackSettings> context)
    {
        var tagData = context.Target.CurrentTagData;
        var directoryFiles = fileScanner
            .Search([context.Target.BackupPath.DirectoryName()])
            .Order()
            .ToList();
        var track = (
            directoryFiles.ToList().FindIndex(x => x == context.Target.BackupPath) + 1
        ).ToString();
        tagData.SetValue(FieldName.Track, track);
        tagData.SetValue(FieldName.TrackTotal, directoryFiles.Count.ToString());
        if (!context.Settings.KeepDisk)
        {
            tagData.RemoveField(FieldName.Disc);
            tagData.RemoveField(FieldName.DiscTotal);
        }

        context.Target.UpdateTagData(tagData);
    }
}
