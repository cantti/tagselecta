using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using TagSelecta.Cli.Tui;

namespace TagSelecta.Cli.Commands.RenameFile;

public static class Registration
{
    public static void AddRenameFile(this IConfigurator configurator, IServiceCollection services)
    {
        services.AddTransient<ITagDataAction, RenameFileAction>();
    }
}
