using Newtonsoft.Json.Linq;

namespace TagSelecta.TagDataActions.Discogs.DiscogsApi;

public interface IDiscogsApi
{
    Task<JToken?> GetRelease(int id, CancellationToken token);

    Task<JToken?> GetMaster(int id, CancellationToken token);
}
