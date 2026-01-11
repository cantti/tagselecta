using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace TagSelecta.App.TagDataActions.AutoTrack;

public static class Registration
{
    public static void AddAutoTrack(this IConfigurator configurator, IServiceCollection services)
    {
        configurator
            .AddTagDataAction<AutoTrackAction>(services, "autotrack")
            .WithDescription("Auto track.");
    }
}
