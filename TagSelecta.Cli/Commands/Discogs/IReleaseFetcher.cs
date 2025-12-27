namespace TagSelecta.Cli.Commands.Discogs;

public interface IReleaseFetcher
{
    Task<ReleaseFetcherResult?> Fetch(DiscogsSettings settings);
}