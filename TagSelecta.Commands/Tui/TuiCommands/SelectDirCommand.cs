using TagSelecta.Shared.IO;

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

        var dir = PathUtils.GetDirectoryName(context.FocusedFile.BackupPath);
        var filesToSelect = context
            .Files.Where(x => PathUtils.GetDirectoryName(x.BackupPath) == dir && !x.IsSelected)
            .ToList();
        foreach (var file in filesToSelect)
        {
            file.IsSelected = true;
        }

        return Task.CompletedTask;
    }
}
