using Microsoft.Extensions.DependencyInjection;
using TagSelecta.Commands.Tui.TuiCommands;

namespace TagSelecta.App.Config;

public static class DependencyInjection
{
    public static IServiceCollection AddSettings(this IServiceCollection services, AppConfig config)
    {
        services.AddSingleton(new MacroSettings() { Macros = config.Macros });
        return services;
    }
}
