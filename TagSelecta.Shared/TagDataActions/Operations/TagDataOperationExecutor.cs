using TagSelecta.Shared.IO;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.Shared.TagDataActions;

public class TagDataOperationExecutor(ITagger tagger, IFileSystem fs) : ITagDataOperationExecutor
{
    public void Write(TagDataOperation operation)
    {
        operation.ResetError();
        try
        {
            PerformWrite(operation);
            operation.UpdateBackup();
        }
        catch (Exception ex)
        {
            operation.MarkError(ex);
        }
    }

    public async Task Execute(
        TagDataOperation operation,
        ITagDataAction action,
        ITagDataActionExecuteContext context,
        CancellationToken token
    )
    {
        operation.ResetError();
        try
        {
            await action.ExecuteAsync(context, token);
        }
        catch (Exception ex)
        {
            operation.MarkError(ex);
        }
    }

    private void PerformWrite(TagDataOperation operation)
    {
        if (!TagDataComparer.AreEqual(operation.GetCurrentTagData(), operation.GetBackupTagData()))
        {
            tagger.WriteTags(operation.GetBackupPath(), operation.GetCurrentTagData());
        }

        if (
            operation.GetCurrentPath() == operation.GetBackupPath()
            || fs.Exists(operation.GetCurrentPath())
        )
        {
            return;
        }

        var destDir = Path.GetDirectoryName(operation.GetCurrentPath())!;

        // create directory with subdirectories
        fs.CreateDirectory(destDir);

        // move audio file
        fs.Move(operation.GetBackupPath(), operation.GetCurrentPath());

        // move other files
        if (!operation.GetMoveOptions().HasFlag(MoveOptions.DoNotMoveOtherFiles))
        {
            var otherFiles = fs.GetFiles(Path.GetDirectoryName(operation.GetBackupPath())!)
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
            !operation.GetMoveOptions().HasFlag(MoveOptions.KeepEmptyDirectories)
            && fs.IsDirectoryEmpty(Path.GetDirectoryName(operation.GetBackupPath())!)
        )
        {
            fs.DeleteDirectory(Path.GetDirectoryName(operation.GetBackupPath())!);
        }
    }
}
