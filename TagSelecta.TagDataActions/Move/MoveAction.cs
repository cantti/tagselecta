using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.TagDataActions.Move;

[TagDataActionName("move", "mv")]
public class MoveAction : TagDataAction<MoveSettings>
{
    protected override void Execute(TagDataActionExecuteContext<MoveSettings> context)
    {
        var dir = Path.GetDirectoryName(context.Target.BackupPath)!;
        var formatter = new TagDataFormatter(
            // todo check logic and document
            // if user edited tags we want to use the new tagdata when moving files
            context.Target.CurrentTagData,
            context.Target.BackupPath
        );
        var newName = formatter.Format(context.Settings.Template);
        var newPath = Path.GetFullPath(newName, dir);
        MoveOptions moveOptions = MoveOptions.None;
        if (context.Settings.KeepEmptyDirectories)
        {
            moveOptions |= MoveOptions.KeepEmptyDirectories;
        }
        if (context.Settings.DoNotMoveOtherFiles)
        {
            moveOptions |= MoveOptions.DoNotMoveOtherFiles;
        }
        context.Target.UpdatePath(newPath, moveOptions);
    }
}
