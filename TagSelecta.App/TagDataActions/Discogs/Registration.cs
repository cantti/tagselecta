using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using TagSelecta.App.Shared;
using TagSelecta.App.Tui;

namespace TagSelecta.App.TagDataActions.Discogs;

public static class Registration
{
    public static void AddDiscogs(this IConfigurator configurator, IServiceCollection services)
    {
        services.AddTransient<IReleaseFetcher, ReleaseFetcher>();
        services.AddTransient<ITagDataAction, DiscogsAction>();
        // configurator
        //     .AddTagDataAction<DiscogsAction>(services, "discogs")
        //     .WithDescription(
        //         "Update album from discogs. You can pass discogs release id (not master) or query to search."
        //     )
        //     .WithExample(
        //         [
        //             "discogs",
        //             "path-to-album",
        //             "-r",
        //             "https://www.discogs.com/release/4202979-King-Tubby-Dub-From-The-Roots",
        //         ]
        //     )
        //     .WithExample(["discogs", "path-to-album", "-q", "King Tubby Dub From The Roots"]);
    }
}
