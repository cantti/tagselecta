using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using TagSelecta.Cli.Commands.Tui;

namespace TagSelecta.Cli.Commands.AutoTrack;

public static class Registration
{
    public static void AddAutoTrack(this IConfigurator configurator, IServiceCollection services)
    {
        // configurator
        //     .AddTagDataAction<AutoTrackAction>(services, "autotrack")
        //     .WithDescription("Auto track.");
        services.AddTransient<ITagDataAction, AutoTrackAction>();
    }
}
