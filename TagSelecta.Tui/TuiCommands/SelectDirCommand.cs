namespace TagSelecta.Tui.TuiCommands;

[TuiCommand("selectdir")]
public class SelectDirCommand : ITuiCommand
{
    public Task ExecuteAsync(ITuiCommandContext context, Request request, CancellationToken token)
    {
        if (context.FocusedFile is null)
        {
            return Task.CompletedTask;
        }
        var dir = Path.GetDirectoryName(context.FocusedFile.BackupPath);
        foreach (
            var file in context
                .Files.Where(x => Path.GetDirectoryName(x.BackupPath) == dir)
                .ToList()
        )
        {
            file.IsSelected = true;
        }
        return Task.CompletedTask;
    }
}
