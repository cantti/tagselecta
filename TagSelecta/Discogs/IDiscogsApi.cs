using Refit;

namespace TagSelecta.Discogs;

[Headers(
    "User-Agent: TagSelecta/1.0 +https://github.com/cantti/tagselecta",
    "Authorization: Discogs"
)]
public interface IDiscogsApi
{
    [Get("/releases/{id}")]
    Task<Release> GetRelease(int id);

    [Get("/masters/{id}")]
    Task<Release> GetMaster(int id);

    [Get("/database/search?type=master")]
    Task<SearchResult> Search(string q);
}
