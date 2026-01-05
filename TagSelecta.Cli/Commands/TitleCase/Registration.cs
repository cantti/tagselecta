using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using TagSelecta.Cli.Tui;

namespace TagSelecta.Cli.Commands.TitleCase;

public static class Registration
{
    public static void AddTitleCase(this IConfigurator configurator, IServiceCollection services)
    {
        // configurator
        //     .AddTagDataAction<TitleCaseAction>(services, "titlecase")
        //     .WithDescription("Convert all fields to title case.");
        services.AddTransient<ITagDataAction, TitleCaseAction>();
    }
}
