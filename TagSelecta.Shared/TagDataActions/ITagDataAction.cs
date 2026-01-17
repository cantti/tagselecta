namespace TagSelecta.Shared.TagDataActions;

public interface ITagDataAction
{
    Task ExecuteAsync(
        ITagDataActionContext current,
        IEnumerable<ITagDataActionContext> files,
        TagDataActionSettings settings,
        CancellationToken token
    );

    Task<bool> BeforeExecuteAsync(TagDataActionSettings settings, CancellationToken token);
}
