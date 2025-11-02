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
    [CommandOption("--query|-q")]
    public string? Query { get; set; }
}

public class DiscogsAction(
    IDiscogsApi discogsApi,
    DiscogsImageDownloader discogsImageDownloader,
    IAnsiConsole console,
    Printer printer
) : FileAction<DiscogsSettings>
{
    private Release? _release;
    private byte[]? _image;

    public override async Task BeforeExecute(ActionBeforeContext<DiscogsSettings> context)
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
        var selectedRelease = releases[promptResult - 1];
        var image = selectedRelease.Images.FirstOrDefault();
        if (image is not null)
        {
            var bytes = await discogsImageDownloader.DownloadAsync(image.Uri);
            _image = bytes;
        }
        _release = selectedRelease;
    }

    public override async Task Execute(ActionContext<DiscogsSettings> context)
    {
        if (_release is null)
            return;
        var tags = Tagger.ReadTags(context.File);
        var track = _release.TrackList[context.FileIndex];
        var albumArtist = _release.Artists.Select(x => x.Name).ToList();
        var artist = track.Artists.Select(x => x.Name).ToList();
        tags.AlbumArtist = albumArtist;
        tags.Artist = artist.Count != 0 ? artist : albumArtist;
        tags.Album = _release.Title;
        tags.Title = track.Title;
        tags.Track = (uint)context.FileIndex + 1;
        tags.TrackTotal = (uint)_release.TrackList.Count;
        tags.Disc = 0;
        tags.DiscTotal = 0;
        tags.Genre = _release.Genres;
        tags.Label = _release.Labels.FirstOrDefault()?.Name ?? "";
        tags.CatalogNumber = _release.Labels.FirstOrDefault()?.CatNo ?? "";
        tags.Year = _release.Year;
        if (_image is not null)
        {
            tags.Pictures = [new TagLib.Picture(_image)];
        }
        printer.PrintTagData(tags);
        if (context.ConfirmPrompt())
        {
            Tagger.WriteTags(context.File, tags);
        }
    }
}
