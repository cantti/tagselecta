using System.Text.RegularExpressions;
using TagSelecta.Cli.Tui;
using TagSelecta.Shared;

namespace TagSelecta.Cli.Commands.Discogs;

[TagDataAction("discogs")]
public class DiscogsAction(IReleaseFetcher releaseFetcher) : TagDataAction<DiscogsSettings>
{
    private ReleaseFetcherResult? _release;
    private List<string> _fieldToWriteList = [];

    protected override async Task<bool> BeforeProcessTagDataAsync(DiscogsSettings settings)
    {
        if (settings.Fields is not null)
        {
            _fieldToWriteList = TagDataActionHelper.NormalizeFieldNames(settings.Fields.ToMulti());
        }

        _release = await releaseFetcher.Fetch(settings);

        return _release is not null;
    }

    protected override void ProcessTagData(
        IFileContext current,
        IEnumerable<IFileContext> files,
        DiscogsSettings settings
    )
    {
        var filesList = files.ToList();

        if (_release is null)
            throw new InvalidOperationException("Release not set");

        var index = filesList.FindIndex(x => x.CurrentPath == current.CurrentPath);
        var track = _release.Release.TrackList[index];

        var albumArtists = _release
            .Release.Artists.Select(a => RemoveTrailingNumberParentheses(a.Name))
            .ToList();
        var trackArtists = track
            .Artists.Select(a => RemoveTrailingNumberParentheses(a.Name))
            .ToList();
        var label = _release.Release.Labels.FirstOrDefault();

        Write(Fields.AlbumArtist, () => current.CurrentTagData.AlbumArtist = albumArtists);
        Write(
            Fields.Artist,
            () =>
                current.CurrentTagData.Artist =
                    trackArtists.Count != 0 ? trackArtists : albumArtists
        );
        Write(Fields.Album, () => current.CurrentTagData.Album = _release.Release.Title);
        Write(Fields.Title, () => current.CurrentTagData.Title = track.Title);
        Write(Fields.Track, () => current.CurrentTagData.Track = track.Position);
        Write(
            Fields.TrackTotal,
            () => current.CurrentTagData.TrackTotal = _release.Release.TrackList.Count.ToString()
        );
        Write(Fields.Disc, () => current.CurrentTagData.Disc = "");
        Write(Fields.DiscTotal, () => current.CurrentTagData.DiscTotal = "");
        Write(Fields.Genre, () => current.CurrentTagData.Genre = _release.Release.Styles);
        Write(Fields.Label, () => current.CurrentTagData.Label = label?.Name ?? "");
        Write(Fields.Date, () => current.CurrentTagData.Date = _release.Release.Year.ToString());
        Write(
            Fields.Picture,
            () => current.CurrentTagData.Picture = [new TagLib.Picture(_release.Image)]
        );
        Write(
            Fields.CatalogNumber,
            () => current.CurrentTagData.CatalogNumber = label?.CatNo ?? ""
        );
        current.CurrentTagData.DiscogsReleaseId = _release.Release.Id.ToString();
    }

    private void Write(string field, Action write)
    {
        if (WriteRequired(field))
        {
            write();
        }
    }

    private bool WriteRequired(string fieldName)
    {
        return _fieldToWriteList.Count == 0
            || _fieldToWriteList.Contains(fieldName.ToLowerInvariant());
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
