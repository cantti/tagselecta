using TagSelecta.Shared.TagDataActions;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.TagDataActions.Move;

[TagDataActionName("move", "mv")]
public class MoveAction : TagDataAction<MoveSettings>
{
    protected override void Execute(
        ITagDataActionContext current,
        IEnumerable<ITagDataActionContext> files,
        MoveSettings settings
    )
    {
        var dir = Path.GetDirectoryName(current.BackupPath)!;
        var formatter = new TagDataFormatter(current.BackupTagData, current.BackupPath);
        var newName = formatter.Format(settings.Template);
        var newPath = Path.GetFullPath(newName, dir);
        MoveOptions moveOptions = MoveOptions.None;
        if (settings.KeepEmptyDirectories)
        {
            moveOptions |= MoveOptions.KeepEmptyDirectories;
        }
        if (settings.DoNotMoveOtherFiles)
        {
            moveOptions |= MoveOptions.DoNotMoveOtherFiles;
        }
        current.SetCurrentPath(newPath, moveOptions);
    }
}
