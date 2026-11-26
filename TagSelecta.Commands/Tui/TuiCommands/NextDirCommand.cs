using TagSelecta.Shared.IO;

namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("nextdir")]
public class NextDirCommand : ITuiCommand
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

        for (var index = context.FocusedFileIndex + 1; index < visibleFiles.Count; index++)
        {
            if (PathUtils.GetDirectoryName(visibleFiles[index].BackupPath) == directory)
            {
                continue;
            }

            context.MetadataOffset = 0;
            context.FocusedFileIndex = index;
            break;
        }

        return Task.CompletedTask;
    }
}
