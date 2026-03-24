namespace TagSelecta.Commands.Github;

public interface IGithubClient
{
    Task<GithubRelease> LatestRelease(CancellationToken ct = default);
}
