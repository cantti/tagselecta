using Refit;
using TagSelecta.TagDataActions.Discogs.DiscogsApi.MasterModels;
using TagSelecta.TagDataActions.Discogs.DiscogsApi.ReleaseModels;

namespace TagSelecta.TagDataActions.Discogs.DiscogsApi;

[Headers(
    "User-Agent: TagSelecta/1.0 +https://github.com/cantti/tagselecta",
    "Authorization: Discogs"
)]
public interface IDiscogsApi
{
    [Get("/releases/{id}")]
    Task<Release> GetRelease(int id);

    [Get("/masters/{id}")]
    Task<Master> GetMaster(int id);
}
