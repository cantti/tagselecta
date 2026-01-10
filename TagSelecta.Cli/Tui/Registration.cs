using Microsoft.Extensions.DependencyInjection;
using TagSelecta.Cli.Tui.TuiCommands;

namespace TagSelecta.Cli.Tui;

public static class Registration
{
    public static void AddCommonTagDataServices(this IServiceCollection services)
    {
        // action factory and dispatcher
        services.AddTransient<ITagDataActionFactory, TagDataActionFactory>();

        services.AddTransient<IUserActionReader, UserActionReader>();
        services.AddSingleton<HotkeyMap>();
        services.AddSingleton<CommandParser>();

        services.AddTransient<ITuiCommand, MoveDownCommand>();
        services.AddTransient<ITuiCommand, MoveUpCommand>();
        services.AddTransient<ITuiCommand, ExecuteTagDataActionCommand>();
        services.AddTransient<ITuiCommand, SelectCommand>();
        services.AddTransient<ITuiCommand, SelectAllCommand>();
        services.AddTransient<ITuiCommand, ClearSelectionCommand>();
        services.AddTransient<ITuiCommand, QuitCommand>();
        services.AddTransient<ITuiCommand, ToggleTreeCommand>();
        services.AddTransient<ITuiCommand, ToggleFilterCommand>();
        services.AddTransient<ITuiCommand, ToggleHelpCommand>();
        services.AddTransient<ITuiCommandFactory, TuiCommandFactory>();
    }
}
