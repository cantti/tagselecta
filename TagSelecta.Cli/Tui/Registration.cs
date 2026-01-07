using Microsoft.Extensions.DependencyInjection;

namespace TagSelecta.Cli.Tui;

public static class Registration
{
    public static void AddCommonTagDataServices(this IServiceCollection services)
    {
        // action factory and dispatcher
        services.AddTransient<ITagDataActionFactory, TagDataActionFactory>();
        services.AddSingleton<ITagDataActionDispatcher, TagDataActionDispatcher>();

        services.AddTransient<IUserActionReader, UserActionReader>();
        services.AddSingleton<HotkeyMap>();
        services.AddSingleton<CommandParser>();
    }
}
