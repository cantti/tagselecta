using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using TagSelecta.Cli.Tui;

namespace TagSelecta.Cli.Commands.Split;

public static class Registration
{
    public static void AddSplit(this IConfigurator configurator, IServiceCollection services)
    {
        // configurator
        //     .AddTagDataAction<SplitAction>(services, "split")
        //     .WithDescription("Split artists, album artists and composers");
        services.AddTransient<ITagDataAction, SplitAction>();
    }
}
