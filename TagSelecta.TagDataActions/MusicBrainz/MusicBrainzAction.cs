using TagSelecta.Shared.Exceptions;
using TagSelecta.Shared.IO;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;
using TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

namespace TagSelecta.TagDataActions.MusicBrainz;

[TagDataActionName("musicbrainz", "mb")]
public class MusicBrainzAction : TagDataAction<MusicBrainzSettings>
{
    private readonly List<MusicBrainzFieldMapEntry> _fieldMap =
    [
        new("album", "{{ release.title }}"),
        new("albumartist", "{{ release.artistcredit | array.map 'name' | joined }}"),
        new(
            "artist",
            "{{ if tracks[index].artistcredit && tracks[index].artistcredit.size > 0; tracks[index].artistcredit | array.map 'name' | joined; else; release.artistcredit | array.map 'name' | joined; end }}"
        ),
        new("catalognumber", "{{ release.labelinfo | array.map 'catalognumber' | joined }}"),
        new("comment", "{{ release.disambiguation }}"),
        new("date", "{{ release.date }}"),
        new("genre", "{{ release.releasegroup.genres | array.map 'name' | joined }}"),
        new("label", "{{ release.labelinfo | array.map 'label' | array.map 'name' | joined }}"),
        new("title", "{{ tracks[index].title }}"),
        new("track", "{{ index + 1 }}"),
        new("tracktotal", "{{ tracks.size }}"),
        new("musicbrainz_release_id", "{{ release.id }}"),
    ];

    private readonly IAudioFileScanner _fileScanner;
    private readonly IMusicBrainzApiClient _musicBrainzApiClient;

    private Release? _release;

    public MusicBrainzAction(
        IMusicBrainzApiClient musicBrainzApiClient,
        IAudioFileScanner fileScanner,
        MusicBrainzConfig musicBrainzConfig
    )
    {
        _musicBrainzApiClient = musicBrainzApiClient;
        _fileScanner = fileScanner;
        MergeFieldMap(musicBrainzConfig.FieldMap);
    }

    public override async Task<bool> BeforeExecuteAsync(
        MusicBrainzSettings settings,
        CancellationToken token
    )
    {
        _release = await _musicBrainzApiClient.GetRelease("10f5fd34-470d-46f8-b364-7e2ddfc765e5");
        return _release is not null;
    }

    protected override void Execute(TagDataActionExecuteContext<MusicBrainzSettings> context)
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
            return;
        }

        foreach (var entry in _fieldMap)
        {
            var value = MusicBrainzTemplateValueResolver.GetValue(
                entry.Value,
                _release,
                trackIndex
            );
            tagData.SetField(entry.FieldName, value);
        }

        context.Target.UpdateTagData(tagData);
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
