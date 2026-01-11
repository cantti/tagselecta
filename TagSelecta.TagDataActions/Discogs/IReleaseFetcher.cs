namespace TagSelecta.TagDataActions.Discogs;

public interface IReleaseFetcher
{
    Task<ReleaseFetcherResult?> Fetch(DiscogsSettings settings);
}
