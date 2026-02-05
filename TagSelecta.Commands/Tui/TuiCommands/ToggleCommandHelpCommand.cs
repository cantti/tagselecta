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
        context.KeymapHelpEnabled = false;
        context.CommandHelpEnabled = !context.CommandHelpEnabled;
        return Task.CompletedTask;
    }
}
