using TagSelecta.TagDataActions.Abstractions;
using TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

namespace TagSelecta.TagDataActions.MusicBrainz;

[TagDataActionName("musicbrainz", "mb")]
public class MusicBrainzAction(IMusicBrainzApiClient musicBrainzApiClient)
    : TagDataAction<MusicBrainzSettings>
{
    public override async Task ExecuteAsync(
        TagDataActionExecuteContext<MusicBrainzSettings> context,
        CancellationToken token
    )
    {
        var release = await musicBrainzApiClient.GetRelease("edf5b60c-4888-4faf-9d6c-a204b84d4e79");
    }
}
