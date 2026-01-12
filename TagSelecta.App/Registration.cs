using Microsoft.Extensions.DependencyInjection;
using Refit;
using Spectre.Console.Cli;
using TagSelecta.CliCommands.Find;
using TagSelecta.Shared.TagDataActions;
using TagSelecta.TagDataActions.AutoTrack;
using TagSelecta.TagDataActions.Discogs;
using TagSelecta.TagDataActions.Discogs.DiscogsApi;
using TagSelecta.TagDataActions.Edit;
using TagSelecta.TagDataActions.ExtractPicture;
using TagSelecta.TagDataActions.Move;
using TagSelecta.TagDataActions.Split;
using TagSelecta.TagDataActions.TitleCase;

namespace TagSelecta.App;

public static class Registration
{
    public static void AddEdit(this IConfigurator configurator, IServiceCollection services)
    {
        configurator
            .AddTagDataAction<EditAction>(services)
            .WithDescription(
                "Edit (read and write) tags. Unrecognized options are saved as custom fields. Another way to edit custom fields is to use --custom option."
            )
            // Basic examples
            .WithExample(
                [
                    "edit",
                    "song.mp3",
                    "-t",
                    "'Song 1'",
                    "-a",
                    "'Artist1;Artist 2'",
                    "-s",
                    "description=test",
                ]
            )
            .WithExample(["edit", "song.mp3", "-c", "'url=https://github.com'"])
            .WithExample(
                [
                    "edit",
                    "song.mp3",
                    "-a",
                    "'{{ artist | regex.replace \"^VA$\" \"Various Artists\" \"-i\" }}'",
                ]
            );
    }

    public static void AddAutoTrack(this IConfigurator configurator, IServiceCollection services)
    {
        configurator.AddTagDataAction<AutoTrackAction>(services).WithDescription("Auto track.");
    }

    public static void AddDiscogs(this IConfigurator configurator, IServiceCollection services)
    {
        services.AddTransient<DiscogsAuthHeaderHandler>();
        services
            .AddRefitClient<IDiscogsApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.discogs.com"))
            .AddHttpMessageHandler<DiscogsAuthHeaderHandler>();
        services
            .AddHttpClient<DiscogsImageDownloader>()
            .AddHttpMessageHandler<DiscogsAuthHeaderHandler>();

        services.AddTransient<IReleaseFetcher, ReleaseFetcher>();
        services.AddTransient<ITagDataAction, DiscogsAction>();
        configurator
            .AddTagDataAction<DiscogsAction>(services)
            .WithDescription(
                "Update album from discogs. You can pass discogs release id (not master) or query to search."
            )
            .WithExample(
                [
                    "discogs",
                    "path-to-album",
                    "-r",
                    "https://www.discogs.com/release/4202979-King-Tubby-Dub-From-The-Roots",
                ]
            )
            .WithExample(["discogs", "path-to-album", "-q", "King Tubby Dub From The Roots"]);
    }

    public static void AddExtractPicture(
        this IConfigurator configurator,
        IServiceCollection services
    )
    {
        configurator
            .AddTagDataAction<ExtractPictureAction>(services)
            .WithDescription("Extract pictures to files.");
    }

    public static void AddMove(this IConfigurator configurator, IServiceCollection services)
    {
        configurator
            .AddTagDataAction<MoveAction>(services)
            .WithDescription("Move (rename) files to another directory.");
    }

    public static void AddSplit(this IConfigurator configurator, IServiceCollection services)
    {
        configurator
            .AddTagDataAction<SplitAction>(services)
            .WithDescription("Split artists, album artists and composers");
    }

    public static void AddTitleCase(this IConfigurator configurator, IServiceCollection services)
    {
        configurator
            .AddTagDataAction<TitleCaseAction>(services)
            .WithDescription("Convert all fields to title case.");
    }

    public static void AddFind(this IConfigurator configurator, IServiceCollection services)
    {
        configurator
            .AddCommand<FindCommand>("find")
            .WithDescription("Find files by metadata")
            .WithExample(
                ["find", ".", "-q", "\"title | string.downcase |  string.contains 'dub'\""]
            );
    }
}
