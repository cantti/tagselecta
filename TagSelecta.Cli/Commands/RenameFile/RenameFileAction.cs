using TagSelecta.Cli.IO;
using TagSelecta.Cli.Tui;

namespace TagSelecta.Cli.Commands.RenameFile;

[TagDataAction("rename", "rn")]
public class RenameFileAction(IFileSystem fs) : TagDataAction<RenameFileSettings>
{
    protected override void ProcessTagData(
        IFileContext current,
        IEnumerable<IFileContext> files,
        RenameFileSettings settings
    )
    {
        current.CurrentPath = FileRenamer.GetNewPath(settings, current);
    }
}
