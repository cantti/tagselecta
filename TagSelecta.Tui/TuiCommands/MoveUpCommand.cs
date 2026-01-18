namespace TagSelecta.Tui.TuiCommands;

[TuiCommand("moveup")]
public class MoveUpCommand : ITuiCommand
{
    public Task ExecuteAsync(ITuiCommandContext context, Request request, CancellationToken token)
    {
        context.FocusedFileIndex = Math.Clamp(
            context.FocusedFileIndex - 1,
            0,
            Math.Max(0, context.VisibleFiles.Count() - 1)
        );
        return Task.CompletedTask;
    }
}
