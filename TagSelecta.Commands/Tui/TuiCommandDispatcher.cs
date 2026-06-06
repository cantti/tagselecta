using TagSelecta.Commands.Tui.TuiCommands;

namespace TagSelecta.Commands.Tui;

public class TuiCommandDispatcher(ITuiCommandFactory commandFactory) : ITuiCommandDispatcher
{
    public async Task<bool> Execute(
        string commandText,
        ITuiCommandContext context,
        CancellationToken token
    )
    {
        if (!CommandParser.TryParse(commandText, out var commands))
        {
            return false;
        }

        foreach (var parsedCommand in commands)
        {
            token.ThrowIfCancellationRequested();
            var command = commandFactory.Create(parsedCommand.Name);
            await command.ExecuteAsync(context, parsedCommand, token);
        }

        return true;
    }
}
