using System.Text.RegularExpressions;
using Spectre.Console;
using TagSelecta.Cli.Commands.Common;
using TagSelecta.Cli.Discogs;
using TagSelecta.Shared;
using TagSelecta.Shared.Exceptions;

namespace TagSelecta.Cli.Commands.Discogs;

public class DiscogsAction(
    IDiscogsApi discogsApi,
    IAnsiConsole console,
    DiscogsImageDownloader discogsImageDownloader
) : TagDataAction<DiscogsSettings>
{
    private Release? _release;
    private byte[]? _image;
    private List<string> _fieldToWriteList = [];

    public override async Task<bool> BeforeProcessTagDataAsync(DiscogsSettings settings)
    {
        if (settings.Fields is not null)
        {
            _fieldToWriteList = TagDataActionHelper.NormalizeFieldNames(settings.Fields.ToMulti());
            if (!TagDataActionHelper.ValidateFieldNameList(console, _fieldToWriteList))
            {
                return false;
            }
        }
        if (settings.Release.StartsWith("http"))
        {
            var (urlType, urlId) = GetDiscogsReleaseInfo(settings.Release);
            var releaseId =
                urlType == "master" ? (await discogsApi.GetMaster(urlId)).MainRelease : urlId;
            _release = await discogsApi.GetRelease(releaseId);
            _release.TrackList = _release.TrackList.Where(x => x.Type == "track").ToList();
            console.MarkupLineInterpolated($"[blue]Release[/]");
            console.MarkupLineInterpolated($"  [blue]Url[/]: [link]{_release.Uri}[/]");
            console.MarkupLineInterpolated(
                $"  [blue]Release[/]: {_release.Artists.Select(x => x.Name).ToJoined()} - {_release.Title} ({_release.Year})"
            );
            console.MarkupLineInterpolated(
                $"  [blue]Tracks[/]: {_release.TrackList.Select((x, i) => $"{i + 1}. {x.Title}").ToJoined()}"
            );
            console.MarkupLineInterpolated($"  [blue]TrackTotal[/]: {_release.TrackList.Count}");
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

    protected override void ProcessTagData(
        TagDataOperation current,
        List<TagDataOperation> operations,
        DiscogsSettings settings
    )
    {
        _release = _release ?? throw new InvalidOperationException("Release not set");

        var track = _release.TrackList[operations.IndexOf(current)];
        var albumArtists = _release
            .Artists.Select(x => RemoveTrailingNumberParentheses(x.Name))
            .ToList();
        var artists = track.Artists.Select(x => RemoveTrailingNumberParentheses(x.Name)).ToList();

        if (WriteRequired(Fields.AlbumArtist))
        {
            current.TagData.AlbumArtist = albumArtists;
        }

        if (WriteRequired(Fields.Artist))
        {
            current.TagData.Artist = artists.Count != 0 ? artists : albumArtists;
        }

        if (WriteRequired(Fields.Album))
        {
            current.TagData.Album = _release.Title;
        }

        if (WriteRequired(Fields.Title))
        {
            current.TagData.Title = track.Title;
        }

        if (WriteRequired(Fields.Track))
        {
            current.TagData.Track = (operations.IndexOf(current) + 1).ToString();
        }

        if (WriteRequired(Fields.TrackTotal))
        {
            current.TagData.TrackTotal = _release.TrackList.Count.ToString();
        }

        if (WriteRequired(Fields.Disc))
        {
            current.TagData.Disc = "";
        }

        if (WriteRequired(Fields.DiscTotal))
        {
            current.TagData.DiscTotal = "";
        }

        if (WriteRequired(Fields.Genre))
        {
            current.TagData.Genre = _release.Styles;
        }

        if (WriteRequired(Fields.Label))
        {
            current.TagData.Label = _release.Labels.FirstOrDefault()?.Name ?? "";
        }

        if (WriteRequired(Fields.Date))
        {
            current.TagData.Date = _release.Year.ToString();
        }

        if (WriteRequired(Fields.Picture))
        {
            current.TagData.Picture = [new TagLib.Picture(_image)];
        }

        if (WriteRequired(Fields.CatalogNumber))
        {
            current.TagData.CatalogNumber = _release.Labels.FirstOrDefault()?.CatNo ?? "";
        }

        current.TagData.DiscogsReleaseId = _release.Id.ToString();
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
