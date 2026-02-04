namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("selectall")]
public class SelectAllCommand : ITuiCommand
{
    public Task ExecuteAsync(ITuiCommandContext context, Request request, CancellationToken token)
    {
        var filesToSelect = context.Files.Where(x => !x.IsSelected).ToList();
        if (filesToSelect.Count > 0)
        {
            foreach (var file in filesToSelect)
            {
                file.IsSelected = true;
            }
        }
        else
        {
            foreach (var file in context.SelectedFiles)
            {
                file.IsSelected = false;
            }
        }
        return Task.CompletedTask;
    }
}
