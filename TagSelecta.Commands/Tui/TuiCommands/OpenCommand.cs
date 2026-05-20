using TagSelecta.Commands.Tui.Completion;
using TagSelecta.Shared.Exceptions;
using TagSelecta.Shared.IO;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("open", "o")]
public class OpenCommand(
    IAudioFileScanner audioFileScanner,
    ITagger tagger,
    ICompletionProvider completionProvider
) : ITuiCommand
{
    public Task ExecuteAsync(
        ITuiCommandContext context,
        ParsedCommand parsedCommand,
        CancellationToken token
    )
    {
        var files = new List<TagDataActionTarget>();
        var inputPaths = parsedCommand.Options.Select(x => Path.GetFullPath(x.Key));
        var paths = audioFileScanner.Search(inputPaths, true);

        for (var i = 0; i < paths.Count; i++)
        {
            var path = paths[i];
            token.ThrowIfCancellationRequested();
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
