using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using TagSelecta.App.Tui;

namespace TagSelecta.App.CliCommands.RenameFile;

public static class Registration
{
    public static void AddRenameFile(this IConfigurator configurator, IServiceCollection services)
    {
        services.AddTransient<ITagDataAction, RenameFileAction>();
    }
}
