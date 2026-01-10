namespace TagSelecta.Cli.CliCommands.Discogs;

public interface IReleaseFetcher
{
    Task<ReleaseFetcherResult?> Fetch(DiscogsSettings settings);
}
