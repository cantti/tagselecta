namespace TagSelecta.Shared.TagDataActions;

public abstract class TagDataAction<TSettings> : ITagDataAction
    where TSettings : TagDataActionSettings
{
    protected virtual bool BeforeProcessTagData(TSettings settings)
    {
        return true;
    }

    public virtual Task<bool> BeforeProcessTagDataAsync(TSettings settings, CancellationToken token)
    {
        return Task.FromResult(BeforeProcessTagData(settings));
    }

    protected virtual void ProcessTagData(
        IFileContext current,
        IEnumerable<IFileContext> files,
        TSettings settings
    ) { }

    public virtual Task ProcessTagDataAsync(
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
        TagDataActionSettings settings,
        CancellationToken token
    )
    {
        return ProcessTagDataAsync(current, files, (TSettings)settings, token);
    }

    Task<bool> ITagDataAction.BeforeProcessTagDataAsync(
        TagDataActionSettings settings,
        CancellationToken token
    )
    {
        return BeforeProcessTagDataAsync((TSettings)settings, token);
    }
}
