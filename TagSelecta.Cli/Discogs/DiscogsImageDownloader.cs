using System.Net.Http.Headers;

namespace TagSelecta.Cli.Discogs;

public class DiscogsImageDownloader(HttpClient client)
{
    public async Task<byte[]> DownloadAsync(string url)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Discogs",
            "key=irBlmropPHHUtaZceGyW, secret=rmnYnuNKxHLxTfVMuJJAjtRRhOuBPQmS"
        );
        var response = await client.GetAsync(url);
        return await response.Content.ReadAsByteArrayAsync();
    }
}
