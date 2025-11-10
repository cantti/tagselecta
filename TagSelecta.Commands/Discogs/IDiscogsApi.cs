using System.ComponentModel.DataAnnotations;
using Refit;

namespace TagSelecta.Commands.Discogs;

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

    [Get("/masters/{id}/versions")]
    Task<MasterVersionList> GetMasterVersions(int id);

    [Get("/database/search")]
    Task<SearchResult> Search(string type, [AliasAs("q")] string query);

    [Get("")]
    Task<HttpResponseMessage> DownloadImage(string url);
}
