namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("select")]
public class SelectCommand : ITuiCommand
{
    public Task ExecuteAsync(ITuiCommandContext context, Request request, CancellationToken token)
    {
        if (context.FocusedFile is null)
        {
            return Task.CompletedTask;
        }
        context.FocusedFile.IsSelected = !context.FocusedFile.IsSelected;
        context.FocusedFileIndex++;
        return Task.CompletedTask;
    }
}
