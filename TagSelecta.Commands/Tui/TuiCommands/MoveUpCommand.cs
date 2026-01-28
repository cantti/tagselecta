namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("moveup")]
public class MoveUpCommand : ITuiCommand
{
    public Task ExecuteAsync(ITuiCommandContext context, Request request, CancellationToken token)
    {
        var newIndex = context.FocusedFileIndex - 1;
        if (newIndex < 0)
        {
            return Task.CompletedTask;
        }
        context.FocusedFileIndex = newIndex;
        return Task.CompletedTask;
    }
}
