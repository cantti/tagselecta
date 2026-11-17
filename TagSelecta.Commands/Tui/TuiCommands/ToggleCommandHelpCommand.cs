namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("togglecommandhelp")]
public class ToggleCommandHelpCommand : ITuiCommand
{
    public Task ExecuteAsync(
        ITuiCommandContext context,
        ParsedCommand parsedCommand,
        CancellationToken token
    )
    {
        context.CommandHelpEnabled = true;
        context.HelpTopic = "";
        return Task.CompletedTask;
    }
}
