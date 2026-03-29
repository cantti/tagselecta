using System.Text.Json;

namespace TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

public interface IMusicBrainzApiClient
{
    Task<JsonElement?> GetRelease(string id, CancellationToken token);
}
