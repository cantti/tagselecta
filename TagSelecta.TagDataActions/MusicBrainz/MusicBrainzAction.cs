using TagSelecta.Shared.Exceptions;
using TagSelecta.Shared.IO;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;
using TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

namespace TagSelecta.TagDataActions.MusicBrainz;

[TagDataActionName("musicbrainz", "mb")]
public class MusicBrainzAction(
    IMusicBrainzApiClient musicBrainzApiClient,
    IAudioFileScanner fileScanner,
    MusicBrainzConfig musicBrainzConfig
) : TagDataAction<MusicBrainzSettings>
{
    private Release? _release;

    public override async Task<bool> BeforeExecuteAsync(
        MusicBrainzSettings settings,
        CancellationToken token
    )
    {
        _release = await musicBrainzApiClient.GetRelease("10f5fd34-470d-46f8-b364-7e2ddfc765e5");
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
        var tracks = MusicBrainzTemplateValueResolver.GetTracks(_release);

        if (trackIndex > tracks.Count - 1)
        {
            return;
        }

        if (trackIndex < 0)
        {
            return;
        }

        foreach (var entry in musicBrainzConfig.FieldMap)
        {
            var value = MusicBrainzTemplateValueResolver.GetValue(entry.Value, _release, trackIndex);
            tagData.SetField(entry.FieldName, value);
        }

        context.Target.UpdateTagData(tagData);
    }
}
