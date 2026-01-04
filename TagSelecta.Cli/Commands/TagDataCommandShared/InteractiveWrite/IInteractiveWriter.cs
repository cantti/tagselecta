namespace TagSelecta.Cli.Commands.TagDataCommandShared.InteractiveWrite;

public interface IInteractiveWriter
{
    void Start(List<TagDataOperation> operations);
}
