using Microsoft.Extensions.DependencyInjection;

namespace TagSelecta.Cli.Commands.Tui;

public static class Registration
{
    public static void AddCommonTagDataServices(this IServiceCollection services)
    {
        services.AddTransient<IUserActionReader, UserActionReader>();
        services.AddSingleton<HotkeyMap>();
        services.AddSingleton<CommandParser>();
        services.AddSingleton<ActionDispatcher>();
    }
}
