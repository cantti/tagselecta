using System.Text.RegularExpressions;
using TagLib;
using TagSelecta.Shared.Exceptions;
using TagSelecta.Shared.IO;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;
using TagSelecta.TagDataActions.Discogs.DiscogsApi;
using TagSelecta.TagDataActions.Discogs.DiscogsApi.ReleaseModels;

namespace TagSelecta.TagDataActions.Discogs;

[TagDataActionInfo("discogs")]
public class DiscogsAction : TagDataAction<DiscogsSettings>
{
    private readonly IDiscogsApi _discogsApi;
    private readonly DiscogsImageDownloader _discogsImageDownloader;

    private readonly List<DiscogsFieldMapEntry> _fieldMap =
    [
        new("album", "{{ release.title }}"),
        new("date", "{{ release.year }}"),
        new("label", "{{ release.labels | array.map 'name' | joined }}"),
        new("catalognumber", "{{ release.labels | array.map 'catno' | joined }}"),
        new("genre", "{{ release.styles | joined }}"),
        new("albumartist", "{{ release.artists | array.map 'name' | joined }}"),
        new(
            "artist",
            "{{ if tracks[index].artists && tracks[index].artists.size > 0; tracks[index].artists | array.map 'name' | joined; else; release.artists | array.map 'name' | joined; end }}"
        ),
        new("title", "{{ tracks[index].title }}"),
        new("track", "{{ index + 1 }}"),
        new("tracktotal", "{{ tracks.size }}"),
        new("discogs_release_id", "{{ release.id }}"),
    ];

    private readonly IAudioFileScanner _fileScanner;

    private Release? _release;
    private byte[]? _releaseImage;

    public DiscogsAction(
        IDiscogsApi discogsApi,
        DiscogsImageDownloader discogsImageDownloader,
        IAudioFileScanner fileScanner,
        DiscogsConfig discogsConfig
    )
    {
        _discogsApi = discogsApi;
        _discogsImageDownloader = discogsImageDownloader;
        _fileScanner = fileScanner;
        MergeFieldMap(discogsConfig.FieldMap);
    }

    public override async Task<bool> BeforeExecuteAsync(
        DiscogsSettings settings,
        CancellationToken token
    )
    {
        var (urlType, urlId) = GetDiscogsReleaseInfo(settings.Url);
        var releaseId = urlId;
        if (urlType == "master")
        {
            var master = await _discogsApi.GetMaster(urlId);
            releaseId = master.MainRelease ?? throw new TagSelectaException("No master release");
        }

        _release = await _discogsApi.GetRelease(releaseId);

        if (_release is null)
        {
            return false;
        }

        var imageUrl = _release.Images?.FirstOrDefault()?.Uri ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(imageUrl))
        {
            _releaseImage = await _discogsImageDownloader.DownloadAsync(imageUrl);
        }

        return true;
    }

    protected override void Execute(TagDataActionExecuteContext<DiscogsSettings> context)
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
        var tracks = DiscogsTemplateValueResolver.GetTracks(_release);

        if (trackIndex > tracks.Count - 1)
        {
            return;
        }

        foreach (var entry in _fieldMap)
        {
            var value = DiscogsTemplateValueResolver.GetValue(entry.Value, _release, trackIndex);
            tagData.SetValue(entry.FieldName, value.SplitTagValuesIfNeeded(entry.FieldName));
        }

        tagData.RemoveField(FieldName.DiscNumber);
        tagData.RemoveField(FieldName.DiscTotal);

        tagData.Picture = [new Picture(_releaseImage)];

        context.Target.UpdateTagData(tagData);
    }

    private static (string Type, int Id) GetDiscogsReleaseInfo(string input)
    {
        var pattern = @"/(release|master)/(\d+)";
        var match = Regex.Match(input, pattern);
        return match.Success
            ? (match.Groups[1].Value, int.Parse(match.Groups[2].Value))
            : throw new TagSelectaException("Error parsing discogs url");
    }

    private void MergeFieldMap(IReadOnlyList<DiscogsFieldMapEntry> overrides)
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
