using Refit;

namespace TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

[Headers(
    "User-Agent: TagSelecta/1.0 +https://github.com/cantti/tagselecta",
    "Accept: application/json"
)]
public interface IMusicBrainzApiClient
{
    [Get("/release/{id}")]
    Task<Release> GetRelease(string id);
}
