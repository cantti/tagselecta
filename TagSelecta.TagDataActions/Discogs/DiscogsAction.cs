using System.Text.RegularExpressions;
using TagSelecta.Shared.Exceptions;
using TagSelecta.Shared.TagDataActions;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.TagDataActions.Discogs;

[TagDataActionName("discogs")]
public class DiscogsAction(IReleaseFetcher releaseFetcher) : TagDataAction<DiscogsSettings>
{
    private ReleaseFetcherResult? _release;
    private List<string> _fieldToWriteList = [];

    public override async Task<bool> BeforeExecuteAsync(
        DiscogsSettings settings,
        CancellationToken token
    )
    {
        if (settings.Fields is not null)
        {
            _fieldToWriteList = settings.Fields.ToMulti().Select(x => x.ToLower().Trim()).ToList();
        }

        _release = await releaseFetcher.Fetch(settings);

        return _release is not null;
    }

    protected override void Execute(TagDataActionExecuteContext<DiscogsSettings> context)
    {
        var tagData = context.Target.CurrentTagData;

        if (_release is null)
        {
            throw new TagSelectaException("Release not set");
        }

        var dir = Path.GetDirectoryName(context.Target.BackupPath);

        var trackNumber = context
            .DirectoryFiles.OrderBy(x => x.Path)
            .ToList()
            .FindIndex(x => x.Path == context.Target.BackupPath);

        if (trackNumber > _release.Release.TrackList?.Count - 1)
        {
            return;
        }

        if (_release.Release.TrackList is null)
        {
            return;
        }

        var track = _release.Release.TrackList[trackNumber];

        var albumArtists =
            _release
                .Release.Artists?.Select(a => RemoveTrailingNumberParentheses(a.Name) ?? "")
                .ToList() ?? [];
        var trackArtists =
            track.Artists?.Select(a => RemoveTrailingNumberParentheses(a.Name) ?? "").ToList()
            ?? [];
        var label = _release.Release.Labels?.FirstOrDefault();

        Write(Fields.AlbumArtist, () => tagData.AlbumArtist = albumArtists);
        Write(
            Fields.Artist,
            () => tagData.Artist = trackArtists.Count != 0 ? trackArtists : albumArtists
        );
        Write(Fields.Album, () => tagData.Album = _release.Release.Title ?? "");
        Write(Fields.Title, () => tagData.Title = track.Title ?? "");
        Write(Fields.Track, () => tagData.Track = track.Position ?? "");
        Write(
            Fields.TrackTotal,
            () => tagData.TrackTotal = _release.Release.TrackList.Count.ToString()
        );
        Write(Fields.Disc, () => tagData.Disc = "");
        Write(Fields.DiscTotal, () => tagData.DiscTotal = "");
        Write(Fields.Genre, () => tagData.Genre = _release.Release.Styles ?? []);
        Write(Fields.Label, () => tagData.Label = label?.Name ?? "");
        Write(Fields.Date, () => tagData.Date = _release.Release.Year.ToString());
        Write(Fields.Picture, () => tagData.Picture = [new TagLib.Picture(_release.Image)]);
        Write(Fields.CatalogNumber, () => tagData.CatalogNumber = label?.CatNo ?? "");
        tagData.SetCustomField("discogs_release_id", _release.Release.Id.ToString());
        context.Target.UpdateTagData(tagData);
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

    private static string? RemoveTrailingNumberParentheses(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        // Remove "(digits)" if it's at the end, possibly with spaces before or after
        var result = Regex.Replace(input, @"\s*\(\d+\)\s*$", "");

        return result.TrimEnd();
    }
}
