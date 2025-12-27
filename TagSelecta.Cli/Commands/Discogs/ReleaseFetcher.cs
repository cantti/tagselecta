using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging.Abstractions;
using Spectre.Console;
using TagSelecta.Cli.Discogs;
using TagSelecta.Shared;
using TagSelecta.Shared.Exceptions;

namespace TagSelecta.Cli.Commands.Discogs;

public class ReleaseFetcher(
    IDiscogsApi discogsApi,
    IAnsiConsole console,
    DiscogsImageDownloader discogsImageDownloader
) : IReleaseFetcher
{
    public async Task<ReleaseFetcherResult?> Fetch(DiscogsSettings settings)
    {
        Release? result;

        if (settings.Release.StartsWith("http"))
        {
            var (urlType, urlId) = GetDiscogsReleaseInfo(settings.Release);
            var releaseId =
                urlType == "master" ? (await discogsApi.GetMaster(urlId)).MainRelease : urlId;
            result = await discogsApi.GetRelease(releaseId);
            result.TrackList = result.TrackList.Where(x => x.Type == "track").ToList();
        }
        else
        {
            var search = await discogsApi.Search("master", settings.Release);
            search.Results = search.Results.Take(5).ToList();
            var releases = new List<Release>();
            var index = -1;
            console.MarkupLineInterpolated($"[green]Discogs releases:[/]");
            console.WriteLine();
            foreach (var searchItem in search.Results)
            {
                index++;
                var master = await discogsApi.GetMaster(searchItem.Id);
                var release = await discogsApi.GetRelease(master.MainRelease);
                release.TrackList = release.TrackList.Where(x => x.Type == "track").ToList();
                releases.Add(release);
                console.MarkupLineInterpolated($"[blue]Option[/] [yellow]{index + 1}[/]");
                console.MarkupLineInterpolated($"  [blue]Url[/]: [link]{release.Uri}[/]");
                console.MarkupLineInterpolated(
                    $"  [blue]Release[/]: {release.Artists.Select(x => x.Name).ToJoined()} - {release.Title} ({release.Year})"
                );
                console.MarkupLineInterpolated(
                    $"  [blue]Tracks[/]: {release.TrackList.Select((x, i) => $"{i + 1}. {x.Title}").ToJoined()}"
                );
                console.MarkupLineInterpolated($"  [blue]TrackTotal[/]: {release.TrackList.Count}");
                console.WriteLine();
            }
            var promptResult = console.Prompt(
                new TextPrompt<int>("Which to choose? (select 0 to exit)")
            );
            if (promptResult == 0)
            {
                return null;
            }
            result = releases[promptResult - 1];
        }

        var image = result.Images.FirstOrDefault();
        byte[]? resultImage = null;
        if (image is not null)
        {
            var bytes = await discogsImageDownloader.DownloadAsync(image.Uri);
            resultImage = bytes;
        }

        console.WriteLine();

        return new ReleaseFetcherResult { Release = result, Image = resultImage };
    }

    private static (string Type, int Id) GetDiscogsReleaseInfo(string input)
    {
        string pattern = @"/(release|master)/(\d+)";
        var match = Regex.Match(input, pattern);
        return match.Success
            ? (match.Groups[1].Value, int.Parse(match.Groups[2].Value))
            : throw new TagSelectaException("Error parsing discogs url");
    }
}

public class ReleaseFetcherResult
{
    public required Release Release { get; set; }
    public required byte[]? Image { get; set; }
}
