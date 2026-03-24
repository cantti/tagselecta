using System.Net.Http.Json;

namespace TagSelecta.Commands.Github;

public class GithubClient : IGithubClient
{
    private static readonly HttpClient HttpClient;

    static GithubClient()
    {
        HttpClient = new HttpClient();
        HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("TagSelecta/1.0");
    }

    public async Task<GithubRelease> LatestRelease(CancellationToken ct = default)
    {
        var release = await HttpClient.GetFromJsonAsync<GithubRelease>(
            "https://api.github.com/repos/cantti/tagselecta/releases/latest",
            ct
        );

        return release ?? new GithubRelease();
    }
}
