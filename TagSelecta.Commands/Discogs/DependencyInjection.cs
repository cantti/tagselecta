using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace TagSelecta.Commands.Discogs;

public static class DependencyInjection
{
    public static IServiceCollection AddDiscogs(this IServiceCollection services)
    {
        services.AddTransient<DiscogsAuthHeaderHandler>();
        services
            .AddRefitClient<IDiscogsApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.discogs.com"))
            .AddHttpMessageHandler<DiscogsAuthHeaderHandler>();
        services
            .AddHttpClient<DiscogsImageDownloader>()
            .AddHttpMessageHandler<DiscogsAuthHeaderHandler>();
        return services;
    }
}
