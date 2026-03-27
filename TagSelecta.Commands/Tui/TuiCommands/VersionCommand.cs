using TagSelecta.Commands.Github;
using TagSelecta.Shared;

namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("version")]
public class VersionCommand(IGithubClient githubClient) : ITuiCommand
{
    public async Task ExecuteAsync(
        ITuiCommandContext context,
        ParsedCommand parsedCommand,
        CancellationToken token
    )
    {
        var resp = await githubClient.LatestRelease(token);
        context.Print($"Current version: {AppVersion.Get()}. Latest version: {resp.TagName}.");
    }
}
