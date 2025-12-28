namespace TagSelecta.Cli.Commands.Common;

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
}
