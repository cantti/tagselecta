namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("selectdir")]
public class SelectDirCommand : ITuiCommand
{
    public Task ExecuteAsync(
        ITuiCommandContext context,
        ParsedCommand parsedCommand,
        CancellationToken token
    )
    {
        if (context.FocusedFile is null)
        {
            return Task.CompletedTask;
        }

        var dir = Path.GetDirectoryName(context.FocusedFile.BackupPath);
        var filesToSelect = context
            .Files.Where(x => Path.GetDirectoryName(x.BackupPath) == dir && !x.IsSelected)
            .ToList();
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
