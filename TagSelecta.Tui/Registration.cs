using Microsoft.Extensions.DependencyInjection;
using TagSelecta.Tui.TuiCommands;

namespace TagSelecta.Tui;

public static class Registration
{
    public static IServiceCollection AddTuiServices(this IServiceCollection services)
    {
        // action factory and dispatcher
        services.AddTransient<ITagDataActionFactory, TagDataActionFactory>();

        services.AddTransient<IRequestReader, RequestReader>();
        services.AddSingleton<HotkeyMap>();
        services.AddSingleton<CommandParser>();

        services.AddTransient<ITuiCommand, MoveDownCommand>();
        services.AddTransient<ITuiCommand, MoveUpCommand>();
        services.AddTransient<ITuiCommand, ExecuteTagDataActionCommand>();
        services.AddTransient<ITuiCommand, SelectCommand>();
        services.AddTransient<ITuiCommand, SelectAllCommand>();
        services.AddTransient<ITuiCommand, ClearSelectionCommand>();
        services.AddTransient<ITuiCommand, SelectDirCommand>();
        services.AddTransient<ITuiCommand, QuitCommand>();
        services.AddTransient<ITuiCommand, ToggleTreeCommand>();
        services.AddTransient<ITuiCommand, ToggleFilterCommand>();
        services.AddTransient<ITuiCommand, ToggleHelpCommand>();
        services.AddTransient<ITuiCommand, WriteCommand>();
        services.AddTransient<ITuiCommand, UndoCommand>();
        services.AddTransient<ITuiCommandFactory, TuiCommandFactory>();

        return services;
    }
}
