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
        var newIndex = context.FocusedFileIndex + 1;
        if (newIndex < context.VisibleFiles.Count())
        {
            context.FocusedFileIndex = newIndex;
        }

        return Task.CompletedTask;
    }
}
