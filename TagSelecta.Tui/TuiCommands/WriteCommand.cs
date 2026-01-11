using TagSelecta.Shared.IO;
using TagSelecta.Tagging;

namespace TagSelecta.Tui.TuiCommands;

[TuiCommand("write", "w")]
public class WriteCommand(ITagger tagger, IFileSystem fs) : ITuiCommand
{
    public Task ExecuteAsync(ITuiCommandContext context, Request request, CancellationToken token)
    {
        var operationsToWrite = context.Operations.Where(x => x.HasChanges).ToList();
        for (var i = 0; i < operationsToWrite.Count; i++)
        {
            var operation = operationsToWrite[i];
            operation.Write(tagger, fs);
            context.Print(
                $"Wrote metadata {i + 1} of {operationsToWrite.Count} ({operation.CurrentPath})"
            );
        }
        return Task.CompletedTask;
    }
}
