namespace TagSelecta.Shared.TagDataActions;

public interface ITagDataActionExecutor
{
    Task Execute(
        TagDataOperation operation,
        int operationIndex,
        ITagDataAction action,
        TagDataActionSettings settings,
        IEnumerable<TagDataOperation> files,
        CancellationToken token
    );
}
