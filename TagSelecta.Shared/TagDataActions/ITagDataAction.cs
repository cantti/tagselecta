namespace TagSelecta.Shared.TagDataActions;

public interface ITagDataAction
{
    Task ProcessTagDataAsync(
        IFileContext current,
        IEnumerable<IFileContext> files,
        TagDataActionSettings settings,
        CancellationToken token
    );

    Task<bool> BeforeProcessTagDataAsync(TagDataActionSettings settings, CancellationToken token);
}
