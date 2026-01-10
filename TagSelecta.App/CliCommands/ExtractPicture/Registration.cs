using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using TagSelecta.App.Tui;

namespace TagSelecta.App.CliCommands.ExtractPicture;

public static class Registration
{
    public static void AddExtractPicture(
        this IConfigurator configurator,
        IServiceCollection services
    )
    {
        // configurator
        //     .AddTagDataAction<ExtractPictureAction>(services, "extractpicture")
        //     .WithDescription("Extract pictures to files.");
        services.AddTransient<ITagDataAction, ExtractPictureAction>();
    }
}
