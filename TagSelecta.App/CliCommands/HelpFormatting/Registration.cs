using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace TagSelecta.App.CliCommands.HelpFormatting;

public static class Registration
{
    public static void AddHelpFormatting(
        this IConfigurator configurator,
        IServiceCollection services
    )
    {
        configurator
            .AddCommand<HelpFormattingCommand>("helpformatting")
            .WithDescription(
                "Show help information about built-in formatting functions and field references."
            );
    }
}
