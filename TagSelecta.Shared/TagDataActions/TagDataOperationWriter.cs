using TagSelecta.Shared.IO;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.Shared.TagDataActions;

public class TagDataOperationWriter(ITagger tagger, IFileSystem fs) : ITagDataOperationWriter
{
    public void Write(TagDataOperation operation)
    {
        if (!TagDataComparer.AreEqual(operation.CurrentTagData, operation.BackupTagData))
        {
            tagger.WriteTags(operation.BackupPath, operation.CurrentTagData);
        }

        if (operation.CurrentPath == operation.BackupPath || fs.Exists(operation.CurrentPath))
        {
            return;
        }

        var destDir = Path.GetDirectoryName(operation.CurrentPath)!;

        // create directory with subdirectories
        fs.CreateDirectory(destDir);

        // move audio file
        fs.Move(operation.BackupPath, operation.CurrentPath);

        // move other files
        if (!operation.MoveOptions.HasFlag(MoveOptions.DoNotMoveOtherFiles))
        {
            var otherFiles = fs.GetFiles(Path.GetDirectoryName(operation.BackupPath)!)
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
            !operation.MoveOptions.HasFlag(MoveOptions.KeepEmptyDirectories)
            && fs.IsDirectoryEmpty(Path.GetDirectoryName(operation.BackupPath)!)
        )
        {
            fs.DeleteDirectory(Path.GetDirectoryName(operation.BackupPath)!);
        }
    }
}
