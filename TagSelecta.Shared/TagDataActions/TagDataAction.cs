namespace TagSelecta.Shared.TagDataActions;

public abstract class TagDataAction<TSettings> : ITagDataAction
    where TSettings : TagDataActionSettings
{
    protected virtual bool BeforeExecute(TSettings settings)
    {
        return true;
    }

    public virtual Task<bool> BeforeExecuteAsync(TSettings settings, CancellationToken token)
    {
        return Task.FromResult(BeforeExecute(settings));
    }

    protected virtual void Execute(
        ITagDataActionContext current,
        IEnumerable<ITagDataActionContext> files,
        TSettings settings
    ) { }

    public virtual Task ExecuteAsync(
        ITagDataActionContext current,
        IEnumerable<ITagDataActionContext> files,
        TSettings settings,
        CancellationToken token
    )
    {
        Execute(current, files, settings);
        return Task.CompletedTask;
    }

    Task ITagDataAction.ExecuteAsync(
        ITagDataActionContext current,
        IEnumerable<ITagDataActionContext> files,
        TagDataActionSettings settings,
        CancellationToken token
    )
    {
        return ExecuteAsync(current, files, (TSettings)settings, token);
    }

    Task<bool> ITagDataAction.BeforeExecuteAsync(
        TagDataActionSettings settings,
        CancellationToken token
    )
    {
        return BeforeExecuteAsync((TSettings)settings, token);
    }
}
