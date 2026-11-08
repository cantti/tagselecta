namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("movestart")]
public class MoveStartCommand : ITuiCommand
{
    public Task ExecuteAsync(
        ITuiCommandContext context,
        ParsedCommand parsedCommand,
        CancellationToken token
    )
    {
        if (context.FocusedFileIndex != 0)
        {
            context.MetadataOffset = 0;
            context.FocusedFileIndex = 0;
        }
        return Task.CompletedTask;
    }
}
