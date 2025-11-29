using Spectre.Console;
using Spectre.Console.Cli;

namespace TagSelecta.Cli.Commands;

public class HelpPictureTypes : CommandSettings { }

public class HelpPictureTypesCommand(IAnsiConsole console) : Command<HelpPictureTypes>
{
    public override int Execute(
        CommandContext context,
        HelpPictureTypes settings,
        CancellationToken cancellationToken
    )
    {
        console.WriteLine(string.Join("\n", Enum.GetNames<TagLib.PictureType>()));
        return 0;
    }
}
