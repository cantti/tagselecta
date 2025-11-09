using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Discogs;
using TagSelecta.Tagging;

namespace TagSelecta.TagDataActions;

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

public class DiscogsAction(
    IDiscogsApi discogsApi,
    IAnsiConsole console,
    DiscogsImageDownloader discogsImageDownloader
) : ITagDataAction<DiscogsSettings>
{
    private Release? _release;
    private byte[]? _image;
    private List<string> _fieldToWriteList = [];

    public async Task<bool> BeforeProcessTagData(TagDataActionContext<DiscogsSettings> context)
    {
        if (context.Settings.Field is not null)
        {
            _fieldToWriteList = context.NormalizeFieldNames(context.Settings.Field);
            if (!context.ValidateFieldNameList(_fieldToWriteList))
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
            _release.TrackList = [.. _release.TrackList.Where(x => x.Type == "track")];
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
            search.Results = [.. search.Results.Take(5)];
            var releases = new List<Release>();
            var index = -1;
            console.MarkupLineInterpolated($"[green]Discogs releases:[/]");
            console.WriteLine();
            foreach (var searchItem in search.Results)
            {
                index++;
                var master = await discogsApi.GetMaster(searchItem.Id);
                var release = await discogsApi.GetRelease(master.MainRelease);
                release.TrackList = [.. release.TrackList.Where(x => x.Type == "track")];
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

    public Task ProcessTagData(TagDataActionContext<DiscogsSettings> context)
    {
        if (_release is null)
            throw new ActionException("Release not set");

        var track = _release.TrackList[context.CurrentFileIndex];
        var albumArtists = _release
            .Artists.Select(x => RemoveTrailingNumberParentheses(x.Name))
            .ToList();
        var artists = track.Artists.Select(x => RemoveTrailingNumberParentheses(x.Name)).ToList();

        SetField(context.TagData, x => x.AlbumArtists, albumArtists);
        SetField(context.TagData, x => x.Artists, artists.Count != 0 ? artists : albumArtists);
        SetField(context.TagData, x => x.Album, _release.Title);
        SetField(context.TagData, x => x.Title, track.Title);
        SetField(context.TagData, x => x.Track, (uint)context.CurrentFileIndex + 1);
        SetField(context.TagData, x => x.TrackTotal, (uint)_release.TrackList.Count);
        SetField(context.TagData, x => x.Disc, (uint)0);
        SetField(context.TagData, x => x.DiscTotal, (uint)0);
        SetField(context.TagData, x => x.Genres, _release.Styles);
        SetField(context.TagData, x => x.Label, _release.Labels.FirstOrDefault()?.Name ?? "");
        SetField(
            context.TagData,
            x => x.CatalogNumber,
            _release.Labels.FirstOrDefault()?.CatNo ?? ""
        );
        SetField(context.TagData, x => x.Year, _release.Year);
        SetField(context.TagData, x => x.DiscogsReleaseId, _release.Id.ToString());
        SetField(context.TagData, x => x.Pictures, [new TagLib.Picture(_image)]);
        return Task.CompletedTask;
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

    private static (string Type, int Id) GetDiscogsReleaseInfo(string input)
    {
        string pattern = @"/(release|master)/(\d+)";
        var match = Regex.Match(input, pattern);
        return match.Success
            ? (match.Groups[1].Value, int.Parse(match.Groups[2].Value))
            : throw new ActionException("Error parsing discogs url");
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
