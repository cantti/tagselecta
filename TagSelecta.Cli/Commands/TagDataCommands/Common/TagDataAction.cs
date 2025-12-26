namespace TagSelecta.Cli.Commands.TagDataCommands.Common;

public abstract class TagDataAction<TSettings>
    where TSettings : BaseSettings
{
    public virtual bool CompareBeforeWriteTagData => true;

    protected virtual bool BeforeProcessTagData(TSettings settings)
    {
        return true;
    }

    public virtual Task<bool> BeforeProcessTagDataAsync(TSettings settings)
    {
        return Task.FromResult(BeforeProcessTagData(settings));
    }

    protected virtual void ProcessTagData(Item current, List<Item> items, TSettings settings) { }

    public virtual Task ProcessTagDataAsync(Item current, List<Item> items, TSettings settings)
    {
        ProcessTagData(current, items, settings);
        return Task.CompletedTask;
    }

    // protected virtual void BeforeWriteTagData(TagDataActionContext<TSettings> context) { }
    //
    // public virtual Task BeforeWriteTagDataAsync(TagDataActionContext<TSettings> context)
    // {
    //     BeforeWriteTagData(context);
    //     return Task.CompletedTask;
    // }
}
