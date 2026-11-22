namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("moveend")]
public class MoveEndCommand : ITuiCommand
{
    public Task ExecuteAsync(
        ITuiCommandContext context,
        ParsedCommand parsedCommand,
        CancellationToken token
    )
    {
        var newIndex = context.VisibleFiles.Count() - 1;
        if (newIndex < 0)
        {
            return Task.CompletedTask;
        }

        if (context.FocusedFileIndex != newIndex)
        {
            context.MetadataOffset = 0;
            context.FocusedFileIndex = newIndex;
        }
        return Task.CompletedTask;
    }
}
