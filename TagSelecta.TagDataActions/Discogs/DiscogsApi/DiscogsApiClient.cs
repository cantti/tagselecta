using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TagSelecta.TagDataActions.Discogs.DiscogsApi;

public class DiscogsApiClient(HttpClient httpClient) : IDiscogsApi
{
    public Task<JToken?> GetRelease(int id, CancellationToken token)
    {
        return GetJson($"releases/{id}", token);
    }

    public Task<JToken?> GetMaster(int id, CancellationToken token)
    {
        return GetJson($"masters/{id}", token);
    }

    private async Task<JToken?> GetJson(string path, CancellationToken token)
    {
        using var response = await httpClient.GetAsync(path, token);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var content = await response.Content.ReadAsStringAsync(token);
        try
        {
            return JToken.Parse(content);
        }
        catch (JsonReaderException ex)
        {
            throw new InvalidOperationException("Discogs response is not valid JSON.", ex);
        }
    }
}
