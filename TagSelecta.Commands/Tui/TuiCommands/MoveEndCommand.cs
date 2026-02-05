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

        context.FocusedFileIndex = newIndex;
        return Task.CompletedTask;
    }
}
