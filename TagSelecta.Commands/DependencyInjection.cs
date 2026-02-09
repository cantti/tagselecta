using Microsoft.Extensions.DependencyInjection;
using TagSelecta.Commands.Tui;

namespace TagSelecta.Commands;

public static class DependencyInjection
{
    public static IServiceCollection AddCommandServices(this IServiceCollection services)
    {
        services.AddTuiServices();
        return services;
    }
}
