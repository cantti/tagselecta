using TagSelecta.Shared.IO;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("open", "o")]
public class OpenCommand(IAudioFileScanner audioFileScanner, ITagger tagger) : ITuiCommand
{
    public Task ExecuteAsync(
        ITuiCommandContext context,
        ParsedCommand parsedCommand,
        CancellationToken token
    )
    {
        var files = new List<TagDataActionTarget>();
        var inputPaths = parsedCommand.Options.Select(x => x.Key);
        var paths = audioFileScanner.Search(inputPaths, true);

        for (var i = 0; i < paths.Count; i++)
        {
            token.ThrowIfCancellationRequested();
            var path = paths[i];
            try
            {
                var tagData = tagger.ReadTags(path);
                files.Add(new TagDataActionTarget(path, tagData));
                context.Print($"Read {i + 1} of {paths.Count} ({path})");
            }
            catch (Exception ex)
            {
                context.Print($"Failed to read {i + 1} of {paths.Count} ({path}): {ex.Message}");
            }
        }

        context.SetFiles(files);
        return Task.CompletedTask;
    }
}
