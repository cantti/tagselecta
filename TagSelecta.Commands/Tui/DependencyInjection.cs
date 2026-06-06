using Microsoft.Extensions.DependencyInjection;
using TagSelecta.Commands.Github;
using TagSelecta.Commands.Tui.Completion;
using TagSelecta.Commands.Tui.TuiCommands;

namespace TagSelecta.Commands.Tui;

public static class DependencyInjection
{
    public static IServiceCollection AddTuiServices(this IServiceCollection services)
    {
        AddTuiCommands(services);
        services.AddTransient<ITagDataActionFactory, TagDataActionFactory>();
        services.AddTransient<ITuiCommandFactory, TuiCommandFactory>();
        services.AddTransient<ITuiCommandDispatcher, TuiCommandDispatcher>();
        services.AddTransient<IGithubClient, GithubClient>();
        services.AddSingleton<ICompletionProvider, CompletionProvider>();
        services.AddTransient<InputHandler>();
        services.AddSingleton<HotkeyMap>();
        return services;
    }

    private static void AddTuiCommands(IServiceCollection services)
    {
        var commandTypes = typeof(ITuiCommand)
            .Assembly.GetTypes()
            .Where(type =>
                type.IsClass && !type.IsAbstract && typeof(ITuiCommand).IsAssignableFrom(type)
            );
        foreach (var commandType in commandTypes)
        {
            services.AddTransient(typeof(ITuiCommand), commandType);
        }
    }
}
