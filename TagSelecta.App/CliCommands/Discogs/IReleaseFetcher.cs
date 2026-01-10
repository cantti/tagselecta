namespace TagSelecta.App.CliCommands.Discogs;

public interface IReleaseFetcher
{
    Task<ReleaseFetcherResult?> Fetch(DiscogsSettings settings);
}
