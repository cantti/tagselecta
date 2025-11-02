using TagSelecta.BaseCommands;

namespace TagSelecta.Actions.Base;

public abstract class FileAction<TSettings>
    where TSettings : FileSettings
{
    public virtual void Configure(ActionConfig cfg) { }

    public virtual Task BeforeExecute(ActionBeforeContext<TSettings> context)
    {
        return Task.CompletedTask;
    }

    public abstract Task Execute(ActionContext<TSettings> context);
}
