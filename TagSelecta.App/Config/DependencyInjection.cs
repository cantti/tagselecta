using Microsoft.Extensions.DependencyInjection;
using TagSelecta.Commands.Tui.TuiCommands;

namespace TagSelecta.App.Config;

public static class DependencyInjection
{
    public static IServiceCollection AddConfig(this IServiceCollection services)
    {
        var config = ConfigReader.Read();
        services.AddSingleton(new MacroSettings() { Macros = config.Macro });
        return services;
    }
}
