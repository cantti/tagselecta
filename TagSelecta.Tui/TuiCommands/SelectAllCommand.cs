namespace TagSelecta.Tui.TuiCommands;

[TuiCommand("selectall")]
public class SelectAllCommand : ITuiCommand
{
    public Task ExecuteAsync(ITuiCommandContext context, Request request, CancellationToken token)
    {
        foreach (var file in context.VisibleFiles)
        {
            file.IsSelected = true;
        }
        return Task.CompletedTask;
    }
}
