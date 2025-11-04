using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Actions.Base;
using TagSelecta.BaseCommands;
using TagSelecta.Discogs;
using TagSelecta.Misc;
using TagSelecta.Tagging;

namespace TagSelecta.Actions;

public class DiscogsSettings : FileSettings
{
    [CommandOption("--release|-r")]
    public string? Release { get; set; }

    [CommandOption("--query|-q")]
    public string? Query { get; set; }

    [CommandOption("--field|-f")]
    [Description(
        "Fields to update from Discogs release. If not specified, all values will be updated"
    )]
    public string[]? Field { get; set; }
}

public class DiscogsAction(
    IDiscogsApi discogsApi,
    DiscogsImageDownloader discogsImageDownloader,
    IAnsiConsole console,
    ActionCommon common,
    ActionContext<DiscogsSettings> context
) : IFileAction<DiscogsSettings>
{
    private Release? _release;
    private byte[]? _image;
    private List<string> _fieldToWriteList = [];

    public async Task BeforeExecute()
    {
        // set field list to write if any
        if (context.Settings.Field is not null)
        {
            _fieldToWriteList = FieldNameValidation.NormalizeFields(context.Settings.Field);
            if (!common.ValidateFieldNameList(_fieldToWriteList))
            {
                context.Cancel();
                return;
            }
        }
        if (context.Settings.Release is not null)
        {
            _release = await discogsApi.GetRelease(GetDiscogsReleaseId(context.Settings.Release));
            _release.TrackList = [.. _release.TrackList.Where(x => x.Type == "track")];
            console.MarkupLineInterpolated($"[blue]Release[/]");
            console.MarkupLineInterpolated($"  [blue]Url[/]: [link]{_release.Uri}[/]");
            console.MarkupLineInterpolated(
                $"  [blue]Release[/]: {_release.Artists.Select(x => x.Name).Print()} - {_release.Title} ({_release.Year})"
            );
            console.MarkupLineInterpolated(
                $"  [blue]Tracks[/]: {_release.TrackList.Select((x, i) => $"{i + 1}. {x.Title}").Print()}"
            );
            console.MarkupLineInterpolated($"  [blue]TrackTotal[/]: {_release.TrackList.Count}");
        }
        else if (!string.IsNullOrWhiteSpace(context.Settings.Query))
        {
            var search = await discogsApi.Search("master", context.Settings.Query ?? "");
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
                    $"  [blue]Release[/]: {release.Artists.Select(x => x.Name).Print()} - {release.Title} ({release.Year})"
                );
                console.MarkupLineInterpolated(
                    $"  [blue]Tracks[/]: {release.TrackList.Select((x, i) => $"{i + 1}. {x.Title}").Print()}"
                );
                console.MarkupLineInterpolated($"  [blue]TrackTotal[/]: {release.TrackList.Count}");
                console.WriteLine();
            }
            var promptResult = AnsiConsole.Prompt(
                new TextPrompt<int>("Which to choose? (select 0 to exit)")
            );
            if (promptResult == 0)
            {
                context.Cancel();
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
            context.Cancel();
        }

        console.WriteLine();
    }

    public async Task Execute(string file, int index)
    {
        if (_release is null)
            return;

        var originalTags = Tagger.ReadTags(file);

        var tags = originalTags.Clone();
        var track = _release.TrackList[index];
        var albumArtist = _release
            .Artists.Select(x => RemoveTrailingNumberParentheses(x.Name))
            .ToList();
        var artist = track.Artists.Select(x => RemoveTrailingNumberParentheses(x.Name)).ToList();

        SetField(tags, x => x.AlbumArtist, albumArtist);
        SetField(tags, x => x.Artist, artist.Count != 0 ? artist : albumArtist);
        SetField(tags, x => x.Album, _release.Title);
        SetField(tags, x => x.Title, track.Title);
        SetField(tags, x => x.Track, (uint)index + 1);
        SetField(tags, x => x.TrackTotal, (uint)_release.TrackList.Count);
        SetField(tags, x => x.Disc, (uint)0);
        SetField(tags, x => x.DiscTotal, (uint)0);
        SetField(tags, x => x.Genre, _release.Styles);
        SetField(tags, x => x.Label, _release.Labels.FirstOrDefault()?.Name ?? "");
        SetField(tags, x => x.CatalogNumber, _release.Labels.FirstOrDefault()?.CatNo ?? "");
        SetField(tags, x => x.Year, _release.Year);
        SetField(tags, x => x.DiscogsReleaseId, _release.Id.ToString());
        SetField(tags, x => x.Picture, [new TagLib.Picture(_image)]);

        if (!common.TagDataChanged(originalTags, tags))
        {
            context.Skip();
            return;
        }

        if (context.ConfirmPrompt())
        {
            Tagger.WriteTags(file, tags);
        }
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

    private static int GetDiscogsReleaseId(string input)
    {
        string pattern = @"/release/(\d+)";
        Match match = Regex.Match(input, pattern);
        return match.Success
            ? int.Parse(match.Groups[1].Value)
            : throw new ActionException("id not found");
    }
}
