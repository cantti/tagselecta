using System.Reflection;
using TagSelecta.Commands.Github;

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
        context.Print($"Current version: {GetAppVersion()}. Latest version: {resp.TagName}.");
    }

    private static string GetAppVersion()
    {
        return Assembly
                .GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion
            ?? "unknown";
    }
}
