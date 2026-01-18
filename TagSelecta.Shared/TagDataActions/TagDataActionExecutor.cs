namespace TagSelecta.Shared.TagDataActions;

public class TagDataActionExecutor : ITagDataActionExecutor
{
    public async Task Execute(
        TagDataOperation operation,
        int operationIndex,
        ITagDataAction action,
        TagDataActionSettings settings,
        IEnumerable<TagDataOperation> files,
        CancellationToken token
    )
    {
        operation.ResetError();
        try
        {
            await action.ExecuteAsync(
                new TagDataActionExecuteContext<TagDataActionSettings>()
                {
                    TargetIndex = operationIndex,
                    Files = files,
                    Settings = settings,
                    Target = operation,
                },
                token
            );
        }
        catch (Exception ex)
        {
            operation.MarkError(ex);
        }
    }
}
