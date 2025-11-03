using TagSelecta.BaseCommands;

namespace TagSelecta.Actions.Base;

public interface IFileAction<TSettings>
    where TSettings : FileSettings
{
    void Configure(ActionConfig cfg) { }

    Task BeforeExecute()
    {
        return Task.CompletedTask;
    }

    Task Execute(string file, int index);
}
