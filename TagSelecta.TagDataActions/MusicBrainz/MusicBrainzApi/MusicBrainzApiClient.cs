using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

public class MusicBrainzApiClient(HttpClient httpClient) : IMusicBrainzApiClient
{
    public async Task<JToken?> GetRelease(string id, CancellationToken token)
    {
        using var response = await httpClient.GetAsync(
            $"release/{Uri.EscapeDataString(id)}?inc=artist-credits+labels+discids+recordings+release-groups+genres&fmt=json",
            token
        );

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
            throw new InvalidOperationException(
                "MusicBrainz response is not valid JSON. Check request URL and response content.",
                ex
            );
        }
    }
}
