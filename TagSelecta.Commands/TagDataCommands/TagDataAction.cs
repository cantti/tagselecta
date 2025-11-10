namespace TagSelecta.Commands.TagDataCommands;

public abstract class TagDataAction<TSettings>
    where TSettings : BaseSettings
{
    public virtual bool CompareBeforeWriteTagData => true;

    protected virtual bool BeforeProcessTagData(TagDataActionContext<TSettings> context)
    {
        return true;
    }

    public virtual Task<bool> BeforeProcessTagDataAsync(TagDataActionContext<TSettings> context)
    {
        return Task.FromResult(BeforeProcessTagData(context));
    }

    protected virtual void ProcessTagData(TagDataActionContext<TSettings> context) { }

    public virtual Task ProcessTagDataAsync(TagDataActionContext<TSettings> context)
    {
        ProcessTagData(context);
        return Task.CompletedTask;
    }

    protected virtual void BeforeWriteTagData(TagDataActionContext<TSettings> context) { }

    public virtual Task BeforeWriteTagDataAsync(TagDataActionContext<TSettings> context)
    {
        BeforeWriteTagData(context);
        return Task.CompletedTask;
    }
}
