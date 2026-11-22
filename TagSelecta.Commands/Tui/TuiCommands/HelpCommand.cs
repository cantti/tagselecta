namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("help", "h")]
public class HelpCommand : ITuiCommand
{
    public Task ExecuteAsync(
        ITuiCommandContext context,
        ParsedCommand parsedCommand,
        CancellationToken token
    )
    {
        context.HelpTopic = parsedCommand.Options.FirstOrDefault()?.Key ?? "";
        context.CommandHelpEnabled = true;
        return Task.CompletedTask;
    }
}
