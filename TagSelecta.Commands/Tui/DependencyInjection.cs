using Microsoft.Extensions.DependencyInjection;
using TagSelecta.Commands.Tui.TuiCommands;

namespace TagSelecta.Commands.Tui;

public static class DependencyInjection
{
    public static IServiceCollection AddTuiServices(this IServiceCollection services)
    {
        services.AddTransient<ITagDataActionFactory, TagDataActionFactory>();
        services.AddTransient<ITuiCommand, MoveDownCommand>();
        services.AddTransient<ITuiCommand, MoveUpCommand>();
        services.AddTransient<ITuiCommand, MoveStartCommand>();
        services.AddTransient<ITuiCommand, MoveEndCommand>();
        services.AddTransient<ITuiCommand, ExecuteTagDataActionCommand>();
        services.AddTransient<ITuiCommand, SelectCommand>();
        services.AddTransient<ITuiCommand, SelectAllCommand>();
        services.AddTransient<ITuiCommand, ClearSelectionCommand>();
        services.AddTransient<ITuiCommand, SelectDirCommand>();
        services.AddTransient<ITuiCommand, QuitCommand>();
        services.AddTransient<ITuiCommand, ToggleTreeCommand>();
        services.AddTransient<ITuiCommand, ToggleFilterCommand>();
        services.AddTransient<ITuiCommand, ToggleKeymapHelpCommand>();
        services.AddTransient<ITuiCommand, ToggleCommandHelpCommand>();
        services.AddTransient<ITuiCommand, WriteCommand>();
        services.AddTransient<ITuiCommand, UndoCommand>();
        services.AddTransient<ITuiCommand, MacroCommand>();
        services.AddTransient<ITuiCommandFactory, TuiCommandFactory>();

        return services;
    }
}
