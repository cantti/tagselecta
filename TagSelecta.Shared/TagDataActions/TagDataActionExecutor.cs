namespace TagSelecta.Shared.TagDataActions;

public class TagDataActionExecutor : ITagDataActionExecutor
{
    public async Task Execute(
        TagDataOperation operation,
        ITagDataAction action,
        ITagDataActionExecuteContext context,
        CancellationToken token
    )
    {
        operation.ResetError();
        try
        {
            await action.ExecuteAsync(context, token);
        }
        catch (Exception ex)
        {
            operation.MarkError(ex);
        }
    }
}
