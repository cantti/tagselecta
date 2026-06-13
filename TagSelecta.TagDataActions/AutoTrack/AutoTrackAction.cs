using TagSelecta.Shared.IO;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.TagDataActions.AutoTrack;

[TagDataActionInfo("autotrack")]
public class AutoTrackAction(IAudioFileScanner fileScanner) : ITagDataAction<AutoTrackSettings>
{
    public Task<bool> BeforeExecute(AutoTrackSettings settings, CancellationToken token)
    {
        return Task.FromResult(true);
    }

    public Task Execute(
        TagDataActionExecuteContext<AutoTrackSettings> context,
        CancellationToken token
    )
    {
        var tagData = context.Target.CurrentTagData;
        var directoryFiles = fileScanner
            .Search([context.Target.BackupPath.DirectoryName()], false)
            .Order()
            .ToList();
        var track = (
            directoryFiles.ToList().FindIndex(x => x == context.Target.BackupPath) + 1
        ).ToString();
        tagData.SetValue(FieldName.TrackNumber, track);
        tagData.SetValue(FieldName.TrackTotal, directoryFiles.Count.ToString());
        if (!context.Settings.KeepDisk)
        {
            tagData.RemoveField(FieldName.DiscNumber);
            tagData.RemoveField(FieldName.DiscTotal);
        }

        context.Target.UpdateTagData(tagData);

        return Task.CompletedTask;
    }
}
