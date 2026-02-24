using Microsoft.Extensions.DependencyInjection;
using TagSelecta.Commands.Tui;
using TagSelecta.Commands.Tui.TuiCommands;

namespace TagSelecta.App.Config;

public static class DependencyInjection
{
    public static IServiceCollection AddSettings(
        this IServiceCollection services,
        ConfigModel configModel
    )
    {
        services.AddSingleton(new MacroConfig { Macros = configModel.Macros });
        services.AddSingleton(
            new TuiAppConfig
            {
                FileListRatio = configModel.General.FileListRatio,
                CompletionEnabled = configModel.General.CompletionEnabled,
            }
        );
        return services;
    }
}
