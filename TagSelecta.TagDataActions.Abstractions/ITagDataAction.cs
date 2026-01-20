namespace TagSelecta.TagDataActions.Abstractions;

public interface ITagDataAction
{
    Task Execute(ITagDataActionExecuteContext context, CancellationToken token);

    Task<bool> BeforeExecute(TagDataActionSettings settings, CancellationToken token);
}
