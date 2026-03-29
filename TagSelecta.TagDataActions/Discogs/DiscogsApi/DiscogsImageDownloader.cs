namespace TagSelecta.TagDataActions.Discogs.DiscogsApi;

public class DiscogsImageDownloader(HttpClient client)
{
    public async Task<byte[]> DownloadAsync(string url)
    {
        var response = await client.GetAsync(url);
        return await response.Content.ReadAsByteArrayAsync();
    }
}
