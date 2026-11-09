using TagSelecta.Commands.Tui.Widgets;

namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("metadataup")]
public class MetadataUpCommand : ITuiCommand
{
    public Task ExecuteAsync(
        ITuiCommandContext context,
        ParsedCommand parsedCommand,
        CancellationToken token
    )
    {
        var maxOffset = context.FocusedFile is not null
            ? Math.Max(0, TagDataRenderer.GetFieldCount(context.FocusedFile) - 1)
            : 0;
        context.MetadataOffset = Math.Max(0, Math.Min(context.MetadataOffset, maxOffset) - 1);
        return Task.CompletedTask;
    }
}
