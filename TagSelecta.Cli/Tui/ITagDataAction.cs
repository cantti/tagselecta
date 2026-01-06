namespace TagSelecta.Cli.Tui;

public interface ITagDataAction
{
    Task ProcessTagDataAsync(
        IFileContext current,
        IEnumerable<IFileContext> files,
        BaseSettings settings
    );

    Task<bool> BeforeProcessTagDataAsync(BaseSettings settings);
}
