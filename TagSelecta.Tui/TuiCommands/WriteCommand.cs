using TagSelecta.Shared.IO;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.Tui.TuiCommands;

[TuiCommand("write", "w")]
public class WriteCommand(ITagger tagger, IFileSystem fileSystem) : ITuiCommand
{
    public Task ExecuteAsync(ITuiCommandContext context, Request request, CancellationToken token)
    {
        var filesToWrite = context.Files.Where(x => x.HasChanges).ToList();
        for (var i = 0; i < filesToWrite.Count; i++)
        {
            token.ThrowIfCancellationRequested();
            var file = filesToWrite[i];
            file.Write(tagger, fileSystem);
            context.Print(
                file.Exception is null
                    ? $"Wrote changes {i + 1} of {filesToWrite.Count} ({file.CurrentPath})"
                    : $"Failed to write metadata {i + 1} of {filesToWrite.Count} ({file.CurrentPath}): {file.Exception.Message}"
            );
        }
        return Task.CompletedTask;
    }
}
