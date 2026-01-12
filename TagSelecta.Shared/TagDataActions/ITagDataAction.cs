namespace TagSelecta.Shared.TagDataActions;

public interface ITagDataAction
{
    Task ExecuteAsync(
        IFileContext current,
        IEnumerable<IFileContext> files,
        TagDataActionSettings settings,
        CancellationToken token
    );

    Task<bool> BeforeExecuteAsync(TagDataActionSettings settings, CancellationToken token);
}
