using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.BaseCommands;
using TagSelecta.Discogs;
using TagSelecta.Tagging;

namespace TagSelecta.Commands;

public class DiscogsSettings : BaseSettings
{
    [CommandOption("--release|-r")]
    public string Release { get; set; } = "";

    [CommandOption("--field|-f")]
    [Description(
        "Fields to update from Discogs release. If not specified, all values will be updated"
    )]
    public string[]? Field { get; set; }

    public override ValidationResult Validate()
    {
        if (string.IsNullOrWhiteSpace(Release))
        {
            return ValidationResult.Error("Release is required");
        }
        return base.Validate();
    }
}

public class DiscogsCommand(
    IDiscogsApi discogsApi,
    DiscogsImageDownloader discogsImageDownloader,
    IAnsiConsole console
) : TagDataProcessingCommand<DiscogsSettings>(console)
{
    private Release? _release;
    private byte[]? _image;
    private List<string> _fieldToWriteList = [];

    protected override async Task BeforeProcessAsync()
    {
        // set field list to write if any
        if (Settings.Field is not null)
        {
            _fieldToWriteList = NormalizeFieldNames(Settings.Field);
            if (!ValidateFieldNameList(_fieldToWriteList))
            {
                Cancel();
                return;
            }
        }
        if (Settings.Release.StartsWith("http"))
        {
            var (urlType, urlId) = GetDiscogsReleaseInfo(Settings.Release);
            var releaseId =
                urlType == "master" ? (await discogsApi.GetMaster(urlId)).MainRelease : urlId;
            _release = await discogsApi.GetRelease(releaseId);
            _release.TrackList = [.. _release.TrackList.Where(x => x.Type == "track")];
            Console.MarkupLineInterpolated($"[blue]Release[/]");
            Console.MarkupLineInterpolated($"  [blue]Url[/]: [link]{_release.Uri}[/]");
            Console.MarkupLineInterpolated(
                $"  [blue]Release[/]: {_release.Artists.Select(x => x.Name).Joined()} - {_release.Title} ({_release.Year})"
            );
            Console.MarkupLineInterpolated(
                $"  [blue]Tracks[/]: {_release.TrackList.Select((x, i) => $"{i + 1}. {x.Title}").Joined()}"
            );
            Console.MarkupLineInterpolated($"  [blue]TrackTotal[/]: {_release.TrackList.Count}");
        }
        else
        {
            var search = await discogsApi.Search("master", Settings.Release);
            search.Results = [.. search.Results.Take(5)];
            var releases = new List<Release>();
            var index = -1;
            Console.MarkupLineInterpolated($"[green]Discogs releases:[/]");
            Console.WriteLine();
            foreach (var searchItem in search.Results)
            {
                index++;
                var master = await discogsApi.GetMaster(searchItem.Id);
                var release = await discogsApi.GetRelease(master.MainRelease);
                release.TrackList = [.. release.TrackList.Where(x => x.Type == "track")];
                releases.Add(release);
                Console.MarkupLineInterpolated($"[blue]Option[/] [yellow]{index + 1}[/]");
                Console.MarkupLineInterpolated($"  [blue]Url[/]: [link]{release.Uri}[/]");
                Console.MarkupLineInterpolated(
                    $"  [blue]Release[/]: {release.Artists.Select(x => x.Name).Joined()} - {release.Title} ({release.Year})"
                );
                Console.MarkupLineInterpolated(
                    $"  [blue]Tracks[/]: {release.TrackList.Select((x, i) => $"{i + 1}. {x.Title}").Joined()}"
                );
                Console.MarkupLineInterpolated($"  [blue]TrackTotal[/]: {release.TrackList.Count}");
                Console.WriteLine();
            }
            var promptResult = Console.Prompt(
                new TextPrompt<int>("Which to choose? (select 0 to exit)")
            );
            if (promptResult == 0)
            {
                Cancel();
                return;
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
            Cancel();
        }

        Console.WriteLine();
    }

    protected override void ProcessTagData()
    {
        if (_release is null)
            return;

        var track = _release.TrackList[CurrentFileIndex];
        var albumArtists = _release
            .Artists.Select(x => RemoveTrailingNumberParentheses(x.Name))
            .ToList();
        var artists = track.Artists.Select(x => RemoveTrailingNumberParentheses(x.Name)).ToList();

        SetField(TagData, x => x.AlbumArtists, albumArtists);
        SetField(TagData, x => x.Artists, artists.Count != 0 ? artists : albumArtists);
        SetField(TagData, x => x.Album, _release.Title);
        SetField(TagData, x => x.Title, track.Title);
        SetField(TagData, x => x.Track, (uint)CurrentFileIndex + 1);
        SetField(TagData, x => x.TrackTotal, (uint)_release.TrackList.Count);
        SetField(TagData, x => x.Disc, (uint)0);
        SetField(TagData, x => x.DiscTotal, (uint)0);
        SetField(TagData, x => x.Genres, _release.Styles);
        SetField(TagData, x => x.Label, _release.Labels.FirstOrDefault()?.Name ?? "");
        SetField(TagData, x => x.CatalogNumber, _release.Labels.FirstOrDefault()?.CatNo ?? "");
        SetField(TagData, x => x.Year, _release.Year);
        SetField(TagData, x => x.DiscogsReleaseId, _release.Id.ToString());
        SetField(TagData, x => x.Pictures, [new TagLib.Picture(_image)]);
    }

    private void SetField<TProp>(
        TagData target,
        Expression<Func<TagData, TProp>> tagDataSelector,
        TProp newValue
    )
    {
        if (tagDataSelector.Body is not MemberExpression memberExpr)
            throw new ArgumentException("Invalid expression form.");
        if (memberExpr.Member is not PropertyInfo propInfo)
            throw new ArgumentException("Expression does not refer to a property.");
        var fieldName = memberExpr.Member.Name.ToLower();
        if (_fieldToWriteList.Count != 0 && !_fieldToWriteList.Contains(fieldName.ToLower()))
            return;
        propInfo.SetValue(target, newValue);
    }

    private static string RemoveTrailingNumberParentheses(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        // Remove "(digits)" if it's at the end, possibly with spaces before or after
        string result = Regex.Replace(input, @"\s*\(\d+\)\s*$", "");

        return result.TrimEnd();
    }

    private static (string Type, int Id) GetDiscogsReleaseInfo(string input)
    {
        string pattern = @"/(release|master)/(\d+)";
        Match match = Regex.Match(input, pattern);
        return match.Success
            ? (match.Groups[1].Value, int.Parse(match.Groups[2].Value))
            : throw new ActionException("Error parsing discogs url");
    }
}
