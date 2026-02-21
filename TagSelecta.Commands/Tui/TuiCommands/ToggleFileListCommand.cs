namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("togglefilelist")]
public class ToggleFileListCommand : ITuiCommand
{
    public Task ExecuteAsync(
        ITuiCommandContext context,
        ParsedCommand parsedCommand,
        CancellationToken token
    )
    {
        context.FileListEnabled = !context.FileListEnabled;
        return Task.CompletedTask;
    }
}
