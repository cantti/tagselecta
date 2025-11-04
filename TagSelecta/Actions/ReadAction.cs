using Spectre.Console;
using TagSelecta.Actions.Base;
using TagSelecta.BaseCommands;
using TagSelecta.Print;
using TagSelecta.Tagging;

namespace TagSelecta.Actions;

public class ReadSettings : FileSettings { }

public class ReadAction(IAnsiConsole console) : IFileAction<ReadSettings>
{
    public void Configure(ActionConfig cfg)
    {
        cfg.ShowContinue = true;
    }

    public Task Execute(string file, int index)
    {
        var tags = Tagger.ReadTags(file);
        Printer.PrintTagData(console, tags);
        return Task.CompletedTask;
    }
}
