using TagSelecta.BaseCommands;

namespace TagSelecta.Actions.Base;

public abstract class FileAction<TSettings>
    where TSettings : FileSettings
{
    public virtual void Configure(ActionConfig cfg) { }

    public abstract void Execute(ActionContext<TSettings> context);
}
