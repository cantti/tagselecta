using System.Net.Http.Headers;

namespace TagSelecta.Cli.Discogs;

public class DiscogsAuthHeaderHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Discogs",
            "key=irBlmropPHHUtaZceGyW, secret=rmnYnuNKxHLxTfVMuJJAjtRRhOuBPQmS"
        );
        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
