namespace TagSelecta.Shared.TagDataActions;

public interface ITagDataOperationExecutor
{
    void Write(TagDataOperation operation);
    Task Execute(
        TagDataOperation operation,
        ITagDataAction action,
        ITagDataActionExecuteContext context,
        CancellationToken token
    );
}
