namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("movedown")]
public class MoveDownCommand : ITuiCommand
{
    public Task ExecuteAsync(ITuiCommandContext context, Request request, CancellationToken token)
    {
        context.FocusedFileIndex = Math.Clamp(
            context.FocusedFileIndex + 1,
            0,
            Math.Max(0, context.VisibleFiles.Count() - 1)
        );
        return Task.CompletedTask;
    }
}
