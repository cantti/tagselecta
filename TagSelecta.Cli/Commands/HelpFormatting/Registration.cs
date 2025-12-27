using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using TagSelecta.Cli.Commands.FixAlbum;

namespace TagSelecta.Cli.Commands.HelpFormatting;

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
