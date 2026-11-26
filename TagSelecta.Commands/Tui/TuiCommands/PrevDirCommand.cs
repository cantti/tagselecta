using TagSelecta.Shared.IO;

namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("prevdir")]
public class PrevDirCommand : ITuiCommand
{
    public Task ExecuteAsync(
        ITuiCommandContext context,
        ParsedCommand parsedCommand,
        CancellationToken token
    )
    {
        var focusedFile = context.FocusedFile;
        if (focusedFile is null)
        {
            return Task.CompletedTask;
        }

        var visibleFiles = context.VisibleFiles.ToList();
        var directory = PathUtils.GetDirectoryName(focusedFile.BackupPath);

        for (var index = context.FocusedFileIndex - 1; index >= 0; index--)
        {
            var previousDirectory = PathUtils.GetDirectoryName(visibleFiles[index].BackupPath);
            if (previousDirectory == directory)
            {
                continue;
            }

            while (
                index > 0
                && PathUtils.GetDirectoryName(visibleFiles[index - 1].BackupPath)
                    == previousDirectory
            )
            {
                index--;
            }

            context.MetadataOffset = 0;
            context.FocusedFileIndex = index;
            break;
        }

        return Task.CompletedTask;
    }
}
