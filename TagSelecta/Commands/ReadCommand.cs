using Spectre.Console;
using TagSelecta.BaseCommands;
using TagSelecta.Print;

namespace TagSelecta.Commands;

public class ReadSettings : FileSettings { }

public class ReadCommand(IAnsiConsole console) : FileCommand<ReadSettings>(console)
{
    protected override void BeforeExecute()
    {
        ShowContinue = true;
    }

    protected override void Execute()
    {
        Printer.PrintTagData(Console, TagData);
    }
}
