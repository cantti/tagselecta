namespace TagSelecta.App.TagDataActions.Discogs;

public interface IReleaseFetcher
{
    Task<ReleaseFetcherResult?> Fetch(DiscogsSettings settings);
}
