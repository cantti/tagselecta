using TagSelecta.App.Tui;

namespace TagSelecta.App.CliCommands.RenameFile;

[TagDataAction("rename", "rn")]
public class RenameFileAction : TagDataAction<RenameFileSettings>
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
