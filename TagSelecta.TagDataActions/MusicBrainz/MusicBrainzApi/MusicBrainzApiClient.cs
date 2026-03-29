using System.Text.Json;

namespace TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

public class MusicBrainzApiClient(HttpClient httpClient) : IMusicBrainzApiClient
{
    public async Task<JsonElement?> GetRelease(string id, CancellationToken token)
    {
        using var response = await httpClient.GetAsync(
            $"release/{Uri.EscapeDataString(id)}?inc=artist-credits+labels+discids+recordings&fmt=json",
            token
        );

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        await using var stream = await response.Content.ReadAsStreamAsync(token);
        try
        {
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: token);
            return document.RootElement.Clone();
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                "MusicBrainz response is not valid JSON. Check request URL and response content.",
                ex
            );
        }
    }
}
