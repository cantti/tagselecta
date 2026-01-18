using TagSelecta.Shared.TrackedFiles;

namespace TagSelecta.Tui.TuiCommands;

[TuiCommand("write", "w")]
public class WriteCommand(ITrackedFileExecutor executor) : ITuiCommand
{
    public Task ExecuteAsync(ITuiCommandContext context, Request request, CancellationToken token)
    {
        var filesToWrite = context.Files.Where(x => x.HasChanges).ToList();
        for (var i = 0; i < filesToWrite.Count; i++)
        {
            token.ThrowIfCancellationRequested();
            var file = filesToWrite[i];
            executor.Write(file);
            context.Print(
                file.Exception is null
                    ? $"Wrote changes {i + 1} of {filesToWrite.Count} ({file.GetCurrentPath()})"
                    : $"Failed to write metadata {i + 1} of {filesToWrite.Count} ({file.GetCurrentPath()}): {file.Exception.Message}"
            );
        }
        return Task.CompletedTask;
    }
}
