using TagSelecta.App.Shared;
using TagSelecta.App.Tui;

namespace TagSelecta.App.TagDataActions.RenameFile;

[TuiTagDataAction("rename", "rn")]
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
