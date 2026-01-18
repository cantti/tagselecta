using TagSelecta.Shared.IO;
using TagSelecta.Shared.TagDataActions;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.Shared.TrackedFiles;

public class TrackedFileExecutor(ITagger tagger, IFileSystem fs) : ITrackedFileExecutor
{
    public void Write(TrackedFile trackedFile)
    {
        trackedFile.ResetError();
        try
        {
            PerformWrite(trackedFile);
            trackedFile.UpdateBackup();
        }
        catch (Exception ex)
        {
            trackedFile.MarkError(ex);
        }
    }

    public async Task Execute(
        TrackedFile trackedFile,
        ITagDataAction action,
        ITagDataActionExecuteContext context,
        CancellationToken token
    )
    {
        trackedFile.ResetError();
        try
        {
            await action.Execute(context, token);
        }
        catch (Exception ex)
        {
            trackedFile.MarkError(ex);
        }
    }

    private void PerformWrite(TrackedFile trackedFile)
    {
        if (
            !TagDataComparer.AreEqual(
                trackedFile.GetCurrentTagData(),
                trackedFile.GetBackupTagData()
            )
        )
        {
            tagger.WriteTags(trackedFile.GetBackupPath(), trackedFile.GetCurrentTagData());
        }

        if (
            trackedFile.GetCurrentPath() == trackedFile.GetBackupPath()
            || fs.Exists(trackedFile.GetCurrentPath())
        )
        {
            return;
        }

        var destDir = Path.GetDirectoryName(trackedFile.GetCurrentPath())!;

        // create directory with subdirectories
        fs.CreateDirectory(destDir);

        // move audio file
        fs.Move(trackedFile.GetBackupPath(), trackedFile.GetCurrentPath());

        // move other files
        if (!trackedFile.GetMoveOptions().HasFlag(MoveOptions.DoNotMoveOtherFiles))
        {
            var otherFiles = fs.GetFiles(Path.GetDirectoryName(trackedFile.GetBackupPath())!)
                .Where(f =>
                    !AudioFileScanner.AllowedExtensions.Contains(Path.GetExtension(f).ToLower())
                );
            foreach (var file in otherFiles)
            {
                var dest = Path.Combine(destDir, Path.GetFileName(file));
                if (!fs.Exists(dest))
                {
                    fs.Move(file, dest);
                }
            }
        }
        // delete empty directories
        if (
            !trackedFile.GetMoveOptions().HasFlag(MoveOptions.KeepEmptyDirectories)
            && fs.IsDirectoryEmpty(Path.GetDirectoryName(trackedFile.GetBackupPath())!)
        )
        {
            fs.DeleteDirectory(Path.GetDirectoryName(trackedFile.GetBackupPath())!);
        }
    }
}
