namespace TagSelecta.Commands.Tui.TuiCommands;

public interface ITuiCommand
{
    Task ExecuteAsync(
        ITuiCommandContext context,
        ParsedCommand parsedCommand,
        CancellationToken token
    );
}
