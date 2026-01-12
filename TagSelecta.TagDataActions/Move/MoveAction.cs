using TagSelecta.Shared.TagDataActions;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.TagDataActions.Move;

[TagDataActionName("move", "mv")]
public class MoveAction : TagDataAction<MoveSettings>
{
    protected override void Execute(
        IFileContext current,
        IEnumerable<IFileContext> files,
        MoveSettings settings
    )
    {
        var dir =
            Path.GetDirectoryName(current.OriginalPath) ?? throw new InvalidOperationException();
        var formatter = new TagDataFormatter(current.OriginalTagData, current.OriginalPath);
        var newName = formatter.Format(settings.Template);
        // if user missed extension, add it
        if (string.IsNullOrEmpty(Path.GetExtension(newName)))
        {
            newName = $"{newName}{Path.GetExtension(current.OriginalPath)}";
        }
        var newPath = Path.GetFullPath(newName, dir);
        current.CurrentPath = newPath;
    }
}
