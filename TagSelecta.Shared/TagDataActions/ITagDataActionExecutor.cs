namespace TagSelecta.Shared.TagDataActions;

public interface ITagDataActionExecutor
{
    Task Execute(
        TagDataOperation operation,
        ITagDataAction action,
        ITagDataActionExecuteContext context,
        CancellationToken token
    );
}
