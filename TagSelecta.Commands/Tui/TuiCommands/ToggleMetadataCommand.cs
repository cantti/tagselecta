namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("togglemetadata")]
public class ToggleMetadataCommand : ITuiCommand
{
    public Task ExecuteAsync(
        ITuiCommandContext context,
        ParsedCommand parsedCommand,
        CancellationToken token
    )
    {
        context.MetadataEnabled = !context.MetadataEnabled;
        return Task.CompletedTask;
    }
}
