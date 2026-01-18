using TagSelecta.Shared.TagDataActions;

namespace TagSelecta.Tui.TuiCommands;

[TuiCommand("write", "w")]
public class WriteCommand(ITagDataOperationWriter writer) : ITuiCommand
{
    public Task ExecuteAsync(ITuiCommandContext context, Request request, CancellationToken token)
    {
        var operationsToWrite = context.Operations.Where(x => x.HasChanges).ToList();
        for (var i = 0; i < operationsToWrite.Count; i++)
        {
            token.ThrowIfCancellationRequested();
            var operation = operationsToWrite[i];
            writer.Write(operation);
            context.Print(
                operation.Exception is null
                    ? $"Wrote changes {i + 1} of {operationsToWrite.Count} ({operation.GetCurrentPath()})"
                    : $"Failed to write metadata {i + 1} of {operationsToWrite.Count} ({operation.GetCurrentPath()}): {operation.Exception.Message}"
            );
        }
        return Task.CompletedTask;
    }
}
