using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Actions.Base;
using TagSelecta.BaseCommands;
using TagSelecta.Discogs;
using TagSelecta.Misc;
using TagSelecta.Print;

namespace TagSelecta.Actions;

public class DiscogsSettings : FileSettings
{
    [CommandOption("--query|-q")]
    public string? Query { get; set; }
}

public class DiscogsAction(IDiscogsApi discogsApi, IAnsiConsole console, Printer printer)
    : FileAction<DiscogsSettings>
{
    private Release? _release;

    public override async Task Execute(ActionContext<DiscogsSettings> context)
    {
        if (_release is null)
        {
            await Search(context);
            if (_release is null)
                return;
        }
    }

    private async Task Search(ActionContext<DiscogsSettings> context)
    {
        var search = await discogsApi.Search(context.Settings.Query ?? "");
        var releases = new List<Release>();
        var index = -1;
        foreach (var searchItem in search.Results.Take(5))
        {
            index++;
            var release = await discogsApi.GetMaster(searchItem.Id);
            releases.Add(release);
            console.MarkupLineInterpolated($"[bold]Option #{index + 1}[/]");
            console.MarkupLineInterpolated($"  [bold]Url[/]: [link]{release.Uri}[/]");
            console.MarkupLineInterpolated(
                $"  [bold]Release[/]: {release.Artists.Select(x => x.Name).Print()} - {release.Title} ({release.Year})"
            );
            console.MarkupLineInterpolated(
                $"  [bold]Tracks[/]: {release.TrackList.Select((x, i) => $"{i + 1}. {x.Title}").Print()}"
            );
            console.MarkupLineInterpolated($"  [bold]TrackTotal[/]: {release.TrackList.Count}");
            console.WriteLine();
        }
        var promptResult = AnsiConsole.Prompt(
            new TextPrompt<int>("Which to choose? (select 0 to exit)")
        );
        if (promptResult == 0)
        {
            context.Cancel();
        }
        else
        {
            _release = releases[promptResult - 1];
        }
    }
}
