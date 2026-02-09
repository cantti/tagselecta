using TagSelecta.Shared.IO;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("write", "w")]
public class WriteCommand(IFileSystem fs, ITagger tagger) : ITuiCommand
{
    public Task ExecuteAsync(
        ITuiCommandContext context,
        ParsedCommand parsedCommand,
        CancellationToken token
    )
    {
        var writer = new TagDataActionTargetWriter(fs, tagger);
        var filesToWrite = context.Files.Where(x => x.HasChanges).ToList();
        for (var i = 0; i < filesToWrite.Count; i++)
        {
            token.ThrowIfCancellationRequested();
            var file = filesToWrite[i];
            writer.Write(file);
            context.Print(
                file.Exception is null
                    ? $"Wrote changes {i + 1} of {filesToWrite.Count} ({file.CurrentPath})"
                    : $"Failed to write metadata {i + 1} of {filesToWrite.Count} ({file.CurrentPath}): {file.Exception.Message}"
            );
        }

        return Task.CompletedTask;
    }
}
