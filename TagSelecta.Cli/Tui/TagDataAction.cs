namespace TagSelecta.Cli.Tui;

public abstract class TagDataAction<TSettings> : ITagDataAction
    where TSettings : BaseSettings
{
    protected virtual bool BeforeProcessTagData(TSettings settings)
    {
        return true;
    }

    public virtual Task<bool> BeforeProcessTagDataAsync(TSettings settings)
    {
        return Task.FromResult(BeforeProcessTagData(settings));
    }

    protected virtual void ProcessTagData(
        FileWithTagData current,
        List<FileWithTagData> files,
        TSettings settings
    ) { }

    public virtual Task ProcessTagDataAsync(
        FileWithTagData current,
        List<FileWithTagData> files,
        TSettings settings
    )
    {
        ProcessTagData(current, files, settings);
        return Task.CompletedTask;
    }

    Task ITagDataAction.ProcessTagDataAsync(
        FileWithTagData current,
        List<FileWithTagData> files,
        BaseSettings settings
    )
    {
        return ProcessTagDataAsync(current, files, (TSettings)settings);
    }

    Task<bool> ITagDataAction.BeforeProcessTagDataAsync(BaseSettings settings)
    {
        return BeforeProcessTagDataAsync((TSettings)settings);
    }
}
