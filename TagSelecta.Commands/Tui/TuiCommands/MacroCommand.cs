using TagLib.Riff;

namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("macro")]
public class MacroCommand(ITuiCommandFactory commandFactory, CommandParser commandParser)
    : ITuiCommand
{
    public async Task ExecuteAsync(
        ITuiCommandContext context,
        Request request,
        CancellationToken token
    )
    {
        var commands = new List<string>() { "edit g=g-from-macro", "edit a=a-from-macro" };

        foreach (var command in commands)
        {
            commandParser.TryParse(command, out var parsedRequest);
            await commandFactory
                .Create(parsedRequest.Name)
                .ExecuteAsync(context, parsedRequest, token);
        }
    }
}
