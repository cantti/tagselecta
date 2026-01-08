namespace TagSelecta.Cli.Tui;

public abstract class TagDataAction<TSettings> : ITagDataAction
    where TSettings : BaseSettings
{
    protected virtual bool BeforeProcessTagData(TSettings settings)
    {
        return true;
    }

    protected virtual Task<bool> BeforeProcessTagDataAsync(
        TSettings settings,
        CancellationToken token
    )
    {
        return Task.FromResult(BeforeProcessTagData(settings));
    }

    protected virtual void ProcessTagData(
        IFileContext current,
        IEnumerable<IFileContext> files,
        TSettings settings
    ) { }

    protected virtual Task ProcessTagDataAsync(
        IFileContext current,
        IEnumerable<IFileContext> files,
        TSettings settings,
        CancellationToken token
    )
    {
        ProcessTagData(current, files, settings);
        return Task.CompletedTask;
    }

    Task ITagDataAction.ProcessTagDataAsync(
        IFileContext current,
        IEnumerable<IFileContext> files,
        BaseSettings settings,
        CancellationToken token
    )
    {
        return ProcessTagDataAsync(current, files, (TSettings)settings, token);
    }

    Task<bool> ITagDataAction.BeforeProcessTagDataAsync(
        BaseSettings settings,
        CancellationToken token
    )
    {
        return BeforeProcessTagDataAsync((TSettings)settings, token);
    }
}
