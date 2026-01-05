using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using TagSelecta.Cli.Commands.Tui;

namespace TagSelecta.Cli.Commands.FixAlbum;

public static class Registration
{
    public static void AddFixAlbum(this IConfigurator configurator, IServiceCollection services)
    {
        // configurator
        //     .AddTagDataAction<FixAlbumAction>(services, "fixalbum")
        //     .WithDescription(
        //         "Set album name, date and album artists to the same value to all files in the same directory."
        //     );
        services.AddTransient<ITagDataAction, FixAlbumAction>();
    }
}
