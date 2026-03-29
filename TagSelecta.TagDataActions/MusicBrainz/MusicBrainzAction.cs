using Newtonsoft.Json.Linq;
using TagSelecta.Shared.Exceptions;
using TagSelecta.Shared.IO;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Json;
using TagSelecta.TagDataActions.Abstractions;
using TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

namespace TagSelecta.TagDataActions.MusicBrainz;

[TagDataActionName("musicbrainz", "mb")]
public class MusicBrainzAction(
    IMusicBrainzApiClient musicBrainzApiClient,
    IAudioFileScanner fileScanner,
    MusicBrainzConfig musicBrainzConfig
)
    : TagDataAction<MusicBrainzSettings>
{
    private JToken? _release;

    public override async Task<bool> BeforeExecuteAsync(
        MusicBrainzSettings settings,
        CancellationToken token
    )
    {
        _release = await musicBrainzApiClient.GetRelease(
            "10f5fd34-470d-46f8-b364-7e2ddfc765e5",
            token
        );
        return _release is not null;
    }

    protected override void Execute(TagDataActionExecuteContext<MusicBrainzSettings> context)
    {
        if (_release is null)
        {
            throw new TagSelectaException("Release not set");
        }

        var tagData = context.Target.CurrentTagData;

        var directoryFiles = fileScanner
            .Search([context.Target.BackupPath.DirectoryName()])
            .Order()
            .ToList();

        var trackIndex = directoryFiles.FindIndex(x => x == context.Target.BackupPath);
        var trackCount = _release.SelectTokens("$.media[*].tracks[*]", false).Count();

        foreach (var entry in musicBrainzConfig.FieldMap)
        {
            var value = GetMappedValue(entry, _release, trackIndex, trackCount);
            tagData.SetField(entry.FieldName, value);
        }

        context.Target.UpdateTagData(tagData);
    }

    private static string GetMappedValue(
        MusicBrainzFieldMapEntry entry,
        JToken release,
        int trackIndex,
        int trackCount
    )
    {
        if (entry.Value.Equals("auto", StringComparison.OrdinalIgnoreCase))
        {
            return GetAutoValue(entry.FieldName, trackIndex, trackCount);
        }

        return entry.PerTrack
            ? JsonPathValueResolver.GetIndexedValue(release, entry.Value, trackIndex)
            : JsonPathValueResolver.GetValue(release, entry.Value);
    }

    private static string GetAutoValue(string fieldName, int trackIndex, int trackCount)
    {
        return fieldName.ToLowerInvariant() switch
        {
            "track" => trackIndex >= 0 ? (trackIndex + 1).ToString() : string.Empty,
            "tracktotal" => trackCount > 0 ? trackCount.ToString() : string.Empty,
            _ => string.Empty,
        };
    }
}
