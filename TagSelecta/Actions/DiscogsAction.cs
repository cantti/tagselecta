using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Actions.Base;
using TagSelecta.BaseCommands;
using TagSelecta.Discogs;
using TagSelecta.Misc;
using TagSelecta.Print;
using TagSelecta.Tagging;

namespace TagSelecta.Actions;

public class DiscogsSettings : FileSettings
{
    [CommandOption("--release|-r")]
    public int? Release { get; set; }

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
    Printer printer,
    ActionContext<DiscogsSettings> context
) : IFileAction<DiscogsSettings>
{
    private Release? _release;
    private byte[]? _image;

    public async Task BeforeExecute()
    {
        if (context.Settings.Release is not null)
        {
            _release = await discogsApi.GetRelease(context.Settings.Release.Value);
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
        var tags = Tagger.ReadTags(file);
        var track = _release.TrackList[index];
        var albumArtist = _release.Artists.Select(x => x.Name).ToList();
        var artist = track.Artists.Select(x => x.Name).ToList();
        SetField(tags, x => x.AlbumArtist, albumArtist, context.Settings.Field);
        SetField(
            tags,
            x => x.Artist,
            artist.Count != 0 ? artist : albumArtist,
            context.Settings.Field
        );
        SetField(tags, x => x.Album, _release.Title, context.Settings.Field);
        SetField(tags, x => x.Title, track.Title, context.Settings.Field);
        SetField(tags, x => x.Track, (uint)index + 1, context.Settings.Field);
        SetField(tags, x => x.TrackTotal, (uint)_release.TrackList.Count, context.Settings.Field);
        SetField(tags, x => x.Disc, (uint)0, context.Settings.Field);
        SetField(tags, x => x.DiscTotal, (uint)0, context.Settings.Field);
        SetField(tags, x => x.Genre, _release.Genres, context.Settings.Field);
        // tags.Label = _release.Labels.FirstOrDefault()?.Name ?? "";
        // tags.CatalogNumber = _release.Labels.FirstOrDefault()?.CatNo ?? "";
        // tags.Year = _release.Year;
        // tags.DiscogsReleaseId = _release.Id.ToString();
        if (_image is not null)
        {
            tags.Pictures = [new TagLib.Picture(_image)];
        }
        printer.PrintTagData(tags);
        if (context.ConfirmPrompt())
        {
            Tagger.WriteTags(file, tags);
        }
    }

    public static void SetField<TProp>(
        TagData target,
        Expression<Func<TagData, TProp>> tagDataSelector,
        TProp newValue,
        string[]? field
    )
    {
        if (tagDataSelector.Body is not MemberExpression memberExpr)
            throw new ArgumentException("Invalid expression form.");
        if (memberExpr.Member is not PropertyInfo propInfo)
            throw new ArgumentException("Expression does not refer to a property.");
        var fieldName = memberExpr.Member.Name.ToLower();
        if (field is not null && !field.Select(x => x.ToLower()).Contains(fieldName.ToLower()))
            return;
        propInfo.SetValue(target, newValue);
    }
}
