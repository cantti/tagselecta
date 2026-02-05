namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("movedown")]
public class MoveDownCommand : ITuiCommand
{
    public Task ExecuteAsync(
        ITuiCommandContext context,
        ParsedCommand parsedCommand,
        CancellationToken token
    )
    {
        var newIndex = context.FocusedFileIndex + 1;
        if (newIndex >= context.VisibleFiles.Count())
        {
            return Task.CompletedTask;
        }

        context.FocusedFileIndex = newIndex;
        return Task.CompletedTask;
    }
}
