namespace TagSelecta.Shared.TagDataActions;

public interface ITagDataAction
{
    Task ExecuteAsync(ITagDataActionExecuteContext context, CancellationToken token);

    Task<bool> BeforeExecuteAsync(TagDataActionSettings settings, CancellationToken token);
}
