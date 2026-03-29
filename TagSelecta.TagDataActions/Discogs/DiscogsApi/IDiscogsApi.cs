namespace TagSelecta.TagDataActions.Discogs.DiscogsApi;

public interface IDiscogsApi
{
    Task<string?> GetRelease(int id, CancellationToken token);

    Task<string?> GetMaster(int id, CancellationToken token);
}
