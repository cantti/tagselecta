using System.Diagnostics;

namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("opendocs")]
public class OpenDocs : ITuiCommand
{
    public Task ExecuteAsync(
        ITuiCommandContext context,
        ParsedCommand parsedCommand,
        CancellationToken token
    )
    {
        Process.Start(
            new ProcessStartInfo
            {
                FileName = "https://cantti.github.io/tagselecta",
                UseShellExecute = true,
            }
        );
        return Task.CompletedTask;
    }
}
