namespace TagSelecta.Cli.Commands.TagDataCommands.Common;

public abstract class TagDataAction<TSettings>
    where TSettings : BaseSettings
{
    public virtual bool CompareBeforeWriteTagData => true;

    protected virtual bool BeforeProcessTagData(ITagDataActionContext<TSettings> context)
    {
        return true;
    }

    public virtual Task<bool> BeforeProcessTagDataAsync(ITagDataActionContext<TSettings> context)
    {
        return Task.FromResult(BeforeProcessTagData(context));
    }

    protected virtual void ProcessTagData(ITagDataActionContext<TSettings> context) { }

    public virtual Task ProcessTagDataAsync(ITagDataActionContext<TSettings> context)
    {
        ProcessTagData(context);
        return Task.CompletedTask;
    }

    protected virtual void BeforeWriteTagData(ITagDataActionContext<TSettings> context) { }

    public virtual Task BeforeWriteTagDataAsync(ITagDataActionContext<TSettings> context)
    {
        BeforeWriteTagData(context);
        return Task.CompletedTask;
    }
}
