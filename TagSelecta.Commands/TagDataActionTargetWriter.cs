using TagSelecta.Shared.IO;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.Commands;

public class TagDataActionTargetWriter(IFileSystem fs, ITagger tagger)
{
    public void Write(TagDataActionTarget target)
    {
        target.ResetError();
        try
        {
            if (!TagDataComparer.AreEqual(target.CurrentTagData, target.BackupTagData))
            {
                tagger.WriteTags(target.BackupPath, target.CurrentTagData);
            }

            if (target.CurrentPath != target.BackupPath && !fs.Exists(target.CurrentPath))
            {
                var destDir = Path.GetDirectoryName(target.CurrentPath)!;

                // create a directory with subdirectories
                fs.CreateDirectory(destDir);

                // move audio file
                fs.Move(target.BackupPath, target.CurrentPath);

                // move other files
                if (!target.MoveOptions.HasFlag(MoveOptions.DoNotMoveOtherFiles))
                {
                    var otherFiles = fs.GetFiles(Path.GetDirectoryName(target.BackupPath)!)
                        .Where(f =>
                            !AudioFileScanner.AllowedExtensions.Contains(
                                Path.GetExtension(f).ToLower()
                            )
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
                    !target.MoveOptions.HasFlag(MoveOptions.KeepEmptyDirectories)
                    && fs.IsDirectoryEmpty(Path.GetDirectoryName(target.BackupPath)!)
                )
                {
                    fs.DeleteDirectory(Path.GetDirectoryName(target.BackupPath)!);
                }
            }

            target.UpdateBackup();
        }
        catch (Exception ex)
        {
            target.MarkError(ex);
        }
    }
}
