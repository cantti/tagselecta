namespace TagSelecta.Cli.Tui;

public interface ITagDataAction
{
    Task ProcessTagDataAsync(
        IFileContext current,
        IEnumerable<IFileContext> files,
        BaseSettings settings,
        CancellationToken token
    );

    Task<bool> BeforeProcessTagDataAsync(BaseSettings settings, CancellationToken token);
}
