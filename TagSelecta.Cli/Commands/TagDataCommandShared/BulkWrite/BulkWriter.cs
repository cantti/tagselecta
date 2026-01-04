using Spectre.Console;
using TagSelecta.Tagging;

namespace TagSelecta.Cli.Commands.TagDataCommandShared.BulkWrite;

public class BulkWriter(IAnsiConsole console, ITagger tagger) : IBulkWriter
{
    public void WriteAll(List<TagDataOperation> operations)
    {
        console.Clear();
        var operationsToWrite = operations
            .Where(x => x is { Status: TagDataOperationStatus.Pending, HasChanges: true })
            .ToList();
        console
            .Progress()
            .Start(ctx =>
            {
                var task = ctx.AddTask("Writing metadata...", maxValue: operationsToWrite.Count);
                for (var i = 0; i < operationsToWrite.Count; i++)
                {
                    var operation = operationsToWrite[i];
                    operation.Write(tagger);
                    task.Description =
                        $"Writing metadata {i + 1} of {operationsToWrite.Count}({operation.Path.EscapeMarkup()})";
                    task.Increment(1);
                }
            });
    }
}
