using Spectre.Console;
using Spectre.Console.Cli;

namespace TagSelecta.Cli.Commands.HelpPictureTypes;

public class HelpPictureTypesCommand(IAnsiConsole console) : Command<HelpPictureTypesSettings>
{
    public override int Execute(
        CommandContext context,
        HelpPictureTypesSettings settings,
        CancellationToken cancellationToken
    )
    {
        console.WriteLine(string.Join("\n", Enum.GetNames<TagLib.PictureType>()));
        return 0;
    }
}
