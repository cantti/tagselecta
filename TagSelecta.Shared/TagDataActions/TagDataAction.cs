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

    protected virtual void Execute(TagDataActionExecuteContext<TSettings> context) { }

    public virtual Task ExecuteAsync(
        TagDataActionExecuteContext<TSettings> context,
        CancellationToken token
    )
    {
        Execute(context);
        return Task.CompletedTask;
    }

    Task ITagDataAction.Execute(ITagDataActionExecuteContext context, CancellationToken token)
    {
        return ExecuteAsync(
            new TagDataActionExecuteContext<TSettings>
            {
                Settings = (TSettings)context.Settings,
                Target = context.Target,
            },
            token
        );
    }

    Task<bool> ITagDataAction.BeforeExecute(TagDataActionSettings settings, CancellationToken token)
    {
        return BeforeExecuteAsync((TSettings)settings, token);
    }
}
