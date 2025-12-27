using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace TagSelecta.Cli.Commands.RenameFile;

public static class Registration
{
    public static void AddRenameFile(this IConfigurator configurator, IServiceCollection services)
    {
        configurator.AddCommand<RenameFileCommand>("renamefile").WithDescription("Rename files.");
    }
}
