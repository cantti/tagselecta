using System.Text.Json;
using System.Text.Json.Nodes;

namespace TagSelecta.TagDataActions.Discogs.DiscogsApi;

public class DiscogsApiClient(HttpClient httpClient) : IDiscogsApi
{
    public Task<string?> GetRelease(int id, CancellationToken token)
    {
        return GetJson($"releases/{id}", token);
    }

    public Task<string?> GetMaster(int id, CancellationToken token)
    {
        return GetJson($"masters/{id}", token);
    }

    private async Task<string?> GetJson(string path, CancellationToken token)
    {
        using var response = await httpClient.GetAsync(path, token);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var content = await response.Content.ReadAsStringAsync(token);
        try
        {
            _ = JsonNode.Parse(content);
            return content;
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Discogs response is not valid JSON.", ex);
        }
    }
}
