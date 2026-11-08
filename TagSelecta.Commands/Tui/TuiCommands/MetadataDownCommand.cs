using TagSelecta.Commands.Tui.Widgets;

namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("metadatadown")]
public class MetadataDownCommand : ITuiCommand
{
    public Task ExecuteAsync(
        ITuiCommandContext context,
        ParsedCommand parsedCommand,
        CancellationToken token
    )
    {
        if (context.FocusedFile is { } file)
        {
            var maxOffset = Math.Max(0, TagDataRenderer.GetFieldCount(file) - 1);
            context.MetadataOffset = Math.Min(context.MetadataOffset + 1, maxOffset);
        }
        return Task.CompletedTask;
    }
}
