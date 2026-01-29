namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("movestart")]
public class MoveStartCommand : ITuiCommand
{
    public Task ExecuteAsync(ITuiCommandContext context, Request request, CancellationToken token)
    {
        context.FocusedFileIndex = 0;
        return Task.CompletedTask;
    }
}
