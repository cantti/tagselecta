using SongTagCli.BaseCommands;

namespace SongTagCli.Actions.Base;

public interface IAction<TSettings>
    where TSettings : FileSettings
{
    void Execute(ActionContext<TSettings> context);
}
