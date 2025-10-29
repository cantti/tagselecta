using TagSelecta.BaseCommands;

namespace TagSelecta.Actions.Base;

public interface IAction<TSettings>
    where TSettings : FileSettings
{
    void Execute(ActionContext<TSettings> context);
}
