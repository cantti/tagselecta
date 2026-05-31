using TagSelecta.Shared.IO;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.TagDataActions.Move;

[TagDataActionInfo("move", "mv")]
public class MoveAction : ITagDataAction<MoveSettings>
{
    public Task<bool> BeforeExecute(MoveSettings settings, CancellationToken token)
    {
        return Task.FromResult(true);
    }

    public Task Execute(TagDataActionExecuteContext<MoveSettings> context, CancellationToken token)
    {
        var dir = PathUtils.GetDirectoryName(context.Target.BackupPath)!;
        var formatter = new TagDataFormatter(
            // todo check logic and document
            // if user edited tags we want to use the new tagdata when moving files
            context.Target.CurrentTagData,
            context.Target.BackupPath,
            true
        );
        var newName = formatter.Format(context.Settings.Template);
        var newPath = PathUtils.GetFullPath(newName, dir);
        var moveOptions = MoveOptions.None;
        if (context.Settings.KeepEmptyDirectories)
        {
            moveOptions |= MoveOptions.KeepEmptyDirectories;
        }

        if (context.Settings.DoNotMoveOtherFiles)
        {
            moveOptions |= MoveOptions.DoNotMoveOtherFiles;
        }

        context.Target.UpdatePath(newPath, moveOptions);

        return Task.CompletedTask;
    }
}
