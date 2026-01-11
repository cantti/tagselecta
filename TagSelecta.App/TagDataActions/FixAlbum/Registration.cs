using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using TagSelecta.App.Shared;
using TagSelecta.App.Tui;

namespace TagSelecta.App.TagDataActions.FixAlbum;

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
