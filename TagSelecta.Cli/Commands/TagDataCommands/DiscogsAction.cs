using System.ComponentModel;
using System.Text.RegularExpressions;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Cli.Discogs;
using TagSelecta.Shared;
using TagSelecta.Shared.Exceptions;
using TagSelecta.Tagging;

namespace TagSelecta.Cli.Commands.TagDataCommands;

public class DiscogsSettings : BaseSettings
{
    [CommandOption("--release|-r")]
    public string Release { get; set; } = "";

    [CommandOption("--fields|-f")]
    [Description(
        "Fields to update from Discogs release. If not specified, all values will be updated"
    )]
    public string? Fields { get; set; }
}

public class DiscogsAction(
    IDiscogsApi discogsApi,
    IAnsiConsole console,
    DiscogsImageDownloader discogsImageDownloader
) : TagDataAction<DiscogsSettings>
{
    private Release? _release;
    private byte[]? _image;
    private List<string> _fieldToWriteList = [];

    public override async Task<bool> BeforeProcessTagDataAsync(
        TagDataActionContext<DiscogsSettings> context
    )
    {
        if (context.Settings.Fields is not null)
        {
            _fieldToWriteList = TagDataActionHelper.NormalizeFieldNames(
                context.Settings.Fields.ToMulti()
            );
            if (!TagDataActionHelper.ValidateFieldNameList(console, _fieldToWriteList))
            {
                return false;
            }
        }
        if (context.Settings.Release.StartsWith("http"))
        {
            var (urlType, urlId) = GetDiscogsReleaseInfo(context.Settings.Release);
            var releaseId =
                urlType == "master" ? (await discogsApi.GetMaster(urlId)).MainRelease : urlId;
            _release = await discogsApi.GetRelease(releaseId);
            _release.TrackList = _release.TrackList.Where(x => x.Type == "track").ToList();
            console.MarkupLineInterpolated($"[blue]Release[/]");
            console.MarkupLineInterpolated($"  [blue]Url[/]: [link]{_release.Uri}[/]");
            console.MarkupLineInterpolated(
                $"  [blue]Release[/]: {_release.Artists.Select(x => x.Name).Joined()} - {_release.Title} ({_release.Year})"
            );
            console.MarkupLineInterpolated(
                $"  [blue]Tracks[/]: {_release.TrackList.Select((x, i) => $"{i + 1}. {x.Title}").Joined()}"
            );
            console.MarkupLineInterpolated($"  [blue]TrackTotal[/]: {_release.TrackList.Count}");
        }
        else
        {
            var search = await discogsApi.Search("master", context.Settings.Release);
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
                    $"  [blue]Release[/]: {release.Artists.Select(x => x.Name).Joined()} - {release.Title} ({release.Year})"
                );
                console.MarkupLineInterpolated(
                    $"  [blue]Tracks[/]: {release.TrackList.Select((x, i) => $"{i + 1}. {x.Title}").Joined()}"
                );
                console.MarkupLineInterpolated($"  [blue]TrackTotal[/]: {release.TrackList.Count}");
                console.WriteLine();
            }
            var promptResult = console.Prompt(
                new TextPrompt<int>("Which to choose? (select 0 to exit)")
            );
            if (promptResult == 0)
            {
                return false;
            }
            _release = releases[promptResult - 1];
        }

        if (_release is not null)
        {
            var image = _release.Images.FirstOrDefault();
            if (image is not null)
            {
                var bytes = await discogsImageDownloader.DownloadAsync(image.Uri);
                _image = bytes;
            }
        }
        else
        {
            return false;
        }

        console.WriteLine();

        return true;
    }

    protected override void ProcessTagData(TagDataActionContext<DiscogsSettings> context)
    {
        _release = _release ?? throw new InvalidOperationException("Release not set");

        var track = _release.TrackList[context.CurrentFileIndex];
        var albumArtists = _release
            .Artists.Select(x => RemoveTrailingNumberParentheses(x.Name))
            .ToList();
        var artists = track.Artists.Select(x => RemoveTrailingNumberParentheses(x.Name)).ToList();

        if (WriteRequired(TagDataFieldNames.AlbumArtist))
        {
            context.TagData.AlbumArtists = albumArtists;
        }

        if (WriteRequired(TagDataFieldNames.Artist))
        {
            context.TagData.Artists = artists.Count != 0 ? artists : albumArtists;
        }

        if (WriteRequired(TagDataFieldNames.Album))
        {
            context.TagData.Album = _release.Title;
        }

        if (WriteRequired(TagDataFieldNames.Title))
        {
            context.TagData.Title = track.Title;
        }

        if (WriteRequired(TagDataFieldNames.Track))
        {
            context.TagData.Track = (context.CurrentFileIndex + 1).ToString();
        }

        if (WriteRequired(TagDataFieldNames.TrackTotal))
        {
            context.TagData.TrackTotal = _release.TrackList.Count.ToString();
        }

        if (WriteRequired(TagDataFieldNames.Disc))
        {
            context.TagData.Disc = "";
        }

        if (WriteRequired(TagDataFieldNames.DiscTotal))
        {
            context.TagData.DiscTotal = "";
        }

        if (WriteRequired(TagDataFieldNames.Genre))
        {
            context.TagData.Genres = _release.Styles;
        }

        if (WriteRequired(TagDataFieldNames.Label))
        {
            context.TagData.Label = _release.Labels.FirstOrDefault()?.Name ?? "";
        }

        if (WriteRequired(TagDataFieldNames.Date))
        {
            context.TagData.Date = _release.Year.ToString();
        }

        if (WriteRequired(TagDataFieldNames.Pictures))
        {
            context.TagData.Pictures = [new TagLib.Picture(_image)];
        }

        if (WriteRequired(TagDataFieldNames.CatalogNumber))
        {
            context.TagData.CatalogNumber = _release.Labels.FirstOrDefault()?.CatNo ?? "";
        }

        context.TagData.DiscogsReleaseId = _release.Id.ToString();
    }

    private bool WriteRequired(string fieldName)
    {
        return _fieldToWriteList.Count == 0 || _fieldToWriteList.Contains(fieldName.ToLower());
    }

    private static (string Type, int Id) GetDiscogsReleaseInfo(string input)
    {
        string pattern = @"/(release|master)/(\d+)";
        var match = Regex.Match(input, pattern);
        return match.Success
            ? (match.Groups[1].Value, int.Parse(match.Groups[2].Value))
            : throw new TagSelectaException("Error parsing discogs url");
    }

    private static string RemoveTrailingNumberParentheses(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        // Remove "(digits)" if it's at the end, possibly with spaces before or after
        string result = Regex.Replace(input, @"\s*\(\d+\)\s*$", "");

        return result.TrimEnd();
    }
}
