using Spectre.Console;
using TagSelecta.BaseCommands;
using TagSelecta.Print;
using TagSelecta.Tagging;

namespace TagSelecta.Commands;

public class ReadSettings : FileSettings { }

public class ReadCommand(IAnsiConsole console) : FileCommand<ReadSettings>(console)
{
    protected override Task BeforeExecute()
    {
        ShowContinue = true;
        return Task.CompletedTask;
    }

    protected override Task Execute(string file, int index)
    {
        var tags = Tagger.ReadTags(file);
        Printer.PrintTagData(Console, tags);
        return Task.CompletedTask;
    }
}
