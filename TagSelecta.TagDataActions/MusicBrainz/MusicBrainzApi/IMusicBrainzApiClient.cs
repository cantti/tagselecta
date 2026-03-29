using Newtonsoft.Json.Linq;

namespace TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

public interface IMusicBrainzApiClient
{
    Task<JToken?> GetRelease(string id, CancellationToken token);
}
