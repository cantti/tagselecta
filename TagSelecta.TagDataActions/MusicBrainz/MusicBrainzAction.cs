using System.Text.RegularExpressions;
using TagLib;
using TagSelecta.Shared.Exceptions;
using TagSelecta.Shared.Http;
using TagSelecta.Shared.IO;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;
using TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

namespace TagSelecta.TagDataActions.MusicBrainz;

[TagDataActionInfo("musicbrainz", "mb")]
public class MusicBrainzAction : ITagDataAction<MusicBrainzSettings>
{
    private readonly IDownloader _downloader;

    private readonly List<MusicBrainzFieldMapEntry> _fieldMap =
    [
        new(FieldName.Album, "{{ release.title }}"),
        new(FieldName.AlbumArtist, "{{ release.artistcredit | array.map 'name' | joined }}"),
        new(
            FieldName.Artist,
            "{{ if tracks[index].artistcredit && tracks[index].artistcredit.size > 0; tracks[index].artistcredit | array.map 'name' | joined; else; release.artistcredit | array.map 'name' | joined; end }}"
        ),
        new(FieldName.Date, "{{ release.date }}"),
        new(FieldName.DiscNumber, "{{ tracks[index].discnumber }}"),
        new(FieldName.DiscTotal, "{{ tracks[index].disctotal }}"),
        new(FieldName.Genre, "{{ release.releasegroup.genres | array.map 'name' | joined }}"),
        new(FieldName.Isrc, "{{ tracks[index].recording.isrcs | joined }}"),
        new(FieldName.Title, "{{ tracks[index].title }}"),
        new(FieldName.TrackNumber, "{{ tracks[index].tracknumber }}"),
        new(FieldName.TrackTotal, "{{ tracks[index].tracktotal }}"),
        new("label", "{{ release.labelinfo | array.map 'label' | array.map 'name' | joined }}"),
        new("catalognumber", "{{ release.labelinfo | array.map 'catalognumber' | joined }}"),
        new(
            "musicbrainz album artist id",
            "{{ release.artistcredit | array.map 'artist' | array.map 'id' | joined }}"
        ),
        new("musicbrainz album id", "{{ release.id }}"),
        new("musicbrainz album release country", "{{ release.country }}"),
        new("musicbrainz album status", "{{ release.status }}"),
        new("musicbrainz album type", "{{ release.releasegroup.primarytype }}"),
        new(
            "musicbrainz artist id",
            "{{ if tracks[index].artistcredit && tracks[index].artistcredit.size > 0; tracks[index].artistcredit | array.map 'artist' | array.map 'id' | joined; else; release.artistcredit | array.map 'artist' | array.map 'id' | joined; end }}"
        ),
        new("musicbrainz release track id", "{{ tracks[index].id }}"),
        new("musicbrainz release group id", "{{ release.releasegroup.id }}"),
        new("barcode", "{{ release.barcode }}"),
    ];

    private readonly IAudioFileScanner _fileScanner;
    private readonly IMusicBrainzApiClient _musicBrainzApiClient;

    private Release? _release;
    private byte[]? _releaseImage;

    public MusicBrainzAction(
        IMusicBrainzApiClient musicBrainzApiClient,
        IAudioFileScanner fileScanner,
        IDownloader downloader,
        MusicBrainzConfig musicBrainzConfig
    )
    {
        _musicBrainzApiClient = musicBrainzApiClient;
        _fileScanner = fileScanner;
        _downloader = downloader;
        MergeFieldMap(musicBrainzConfig.FieldMap);
    }

    public async Task<bool> BeforeExecute(
        MusicBrainzSettings settings,
        CancellationToken token
    )
    {
        var releaseId = GetReleaseId(settings.Release);
        _release = await _musicBrainzApiClient.GetRelease(releaseId);

        if (_release?.CoverArtArchive?.Front == true)
        {
            var coverUrl = $"https://coverartarchive.org/release/{releaseId}/front";
            try
            {
                _releaseImage = await _downloader.Download(coverUrl, token);
            }
            catch
            {
                _releaseImage = null;
            }
        }

        return _release is not null;
    }

    private static string GetReleaseId(string release)
    {
        if (Guid.TryParse(release, out var guid))
        {
            return guid.ToString();
        }

        var pattern = @"/release/([\d\w-]+)";
        var match = Regex.Match(release, pattern);
        if (!match.Success)
        {
            throw new TagSelectaException("Error parsing MusicBrainz release");
        }

        return match.Groups[1].Value;
    }

    public Task Execute(TagDataActionExecuteContext<MusicBrainzSettings> context, CancellationToken token)
    {
        if (_release is null)
        {
            throw new TagSelectaException("Release not set");
        }

        var tagData = context.Target.CurrentTagData;

        var directoryFiles = _fileScanner
            .Search([context.Target.BackupPath.DirectoryName()])
            .Order()
            .ToList();

        var trackIndex = directoryFiles.FindIndex(x => x == context.Target.BackupPath);
        var tracks = MusicBrainzTemplateValueResolver.GetTracks(_release);

        if (trackIndex < 0 || trackIndex > tracks.Count - 1)
        {
            return Task.CompletedTask;
        }

        foreach (var entry in _fieldMap)
        {
            var value = MusicBrainzTemplateValueResolver.GetValue(
                entry.Value,
                _release,
                trackIndex
            );
            tagData.SetValue(entry.FieldName, value.SplitTagValuesIfNeeded(entry.FieldName));
        }

        if (_releaseImage is { Length: > 0 })
        {
            var cover = new Picture(_releaseImage) { Type = PictureType.FrontCover };
            tagData.Picture = [cover];
        }

        context.Target.UpdateTagData(tagData);

        return Task.CompletedTask;
    }

    private void MergeFieldMap(IReadOnlyList<MusicBrainzFieldMapEntry> overrides)
    {
        foreach (var entry in overrides)
        {
            var index = _fieldMap.FindIndex(x =>
                x.FieldName.Equals(entry.FieldName, StringComparison.OrdinalIgnoreCase)
            );

            if (index >= 0)
            {
                _fieldMap[index] = entry;
                continue;
            }

            _fieldMap.Add(entry);
        }
    }
}
