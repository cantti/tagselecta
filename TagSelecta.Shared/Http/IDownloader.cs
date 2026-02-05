namespace TagSelecta.Shared.Http;

public interface IDownloader
{
    Task<byte[]> Download(string url, CancellationToken ct = default);
}
