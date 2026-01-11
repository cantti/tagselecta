using TagSelecta.Shared.TagDataActions;

namespace TagSelecta.TagDataActions.RenameFile;

[TagDataActionName("rename")]
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
