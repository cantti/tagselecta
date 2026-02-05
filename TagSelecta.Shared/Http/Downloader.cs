namespace TagSelecta.Shared.Http;

public class Downloader : IDownloader
{
    private static readonly HttpClient HttpClient = new();

    public async Task<byte[]> Download(string url, CancellationToken ct = default)
    {
        return await HttpClient.GetByteArrayAsync(url, ct);
    }
}
