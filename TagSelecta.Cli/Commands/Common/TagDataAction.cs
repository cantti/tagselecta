namespace TagSelecta.Cli.Commands.Common;

public abstract class TagDataAction<TSettings>
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
        TagDataOperation current,
        List<TagDataOperation> operations,
        TSettings settings
    ) { }

    public virtual Task ProcessTagDataAsync(
        TagDataOperation current,
        List<TagDataOperation> items,
        TSettings settings
    )
    {
        ProcessTagData(current, items, settings);
        return Task.CompletedTask;
    }
}
