namespace TagSelecta.Cli.Tui;

public interface ITagDataAction
{
    Task ProcessTagDataAsync(
        FileWithTagData current,
        List<FileWithTagData> files,
        BaseSettings settings
    );

    Task<bool> BeforeProcessTagDataAsync(BaseSettings settings);
}
