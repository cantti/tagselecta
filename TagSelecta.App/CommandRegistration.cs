using Microsoft.Extensions.DependencyInjection;
using Refit;
using Spectre.Console.Cli;
using TagSelecta.Commands;
using TagSelecta.Commands.Cli.Find;
using TagSelecta.Commands.Tui;
using TagSelecta.TagDataActions.AutoTrack;
using TagSelecta.TagDataActions.Discogs;
using TagSelecta.TagDataActions.Discogs.DiscogsApi;
using TagSelecta.TagDataActions.Edit;
using TagSelecta.TagDataActions.ExtractPicture;
using TagSelecta.TagDataActions.Move;
using TagSelecta.TagDataActions.Split;
using TagSelecta.TagDataActions.TitleCase;

namespace TagSelecta.App;

public static class CommandRegistration
{
    public static void AddCommands(this IConfigurator configurator, IServiceCollection services)
    {
        AddEdit(configurator, services);
        AddExtractPicture(configurator, services);
        AddTitleCase(configurator, services);
        AddSplit(configurator, services);
        AddDiscogs(configurator, services);
        AddAutoTrack(configurator, services);
        AddMove(configurator, services);
        AddFind(configurator, services);
        AddTui(configurator, services);
    }

    private static void AddEdit(IConfigurator configurator, IServiceCollection services)
    {
        configurator
            .AddTagDataAction<EditAction>(services)
            .WithDescription(
                "Edit (read and write) tags. To edit extra fields is to use --extra option."
            )
            // Basic examples
            .WithExample(
                "edit",
                "song.mp3",
                "-t",
                "'Song 1'",
                "-a",
                "'Artist1;Artist 2'",
                "-s",
                "description=test"
            )
            .WithExample("edit", "song.mp3", "-c", "'url=https://github.com'")
            .WithExample(
                "edit",
                "song.mp3",
                "-a",
                "'{{ artist | regex.replace \"^VA$\" \"Various Artists\" \"-i\" }}'"
            );
    }

    private static void AddAutoTrack(IConfigurator configurator, IServiceCollection services)
    {
        configurator.AddTagDataAction<AutoTrackAction>(services).WithDescription("Auto track.");
    }

    private static void AddDiscogs(IConfigurator configurator, IServiceCollection services)
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
        configurator
            .AddTagDataAction<DiscogsAction>(services)
            .WithDescription(
                "Update album from discogs. You can pass discogs release id (not master) or query to search."
            )
            .WithExample(
                "discogs",
                "path-to-album",
                "-r",
                "https://www.discogs.com/release/4202979-King-Tubby-Dub-From-The-Roots"
            )
            .WithExample("discogs", "path-to-album", "-q", "King Tubby Dub From The Roots");
    }

    private static void AddExtractPicture(IConfigurator configurator, IServiceCollection services)
    {
        configurator
            .AddTagDataAction<ExtractPictureAction>(services)
            .WithDescription("Extract pictures to files.");
    }

    private static void AddMove(IConfigurator configurator, IServiceCollection services)
    {
        configurator
            .AddTagDataAction<MoveAction>(services)
            .WithDescription("Move (rename) files to another directory.");
    }

    private static void AddSplit(IConfigurator configurator, IServiceCollection services)
    {
        configurator
            .AddTagDataAction<SplitAction>(services)
            .WithDescription("Split artists, album artists and composers");
    }

    private static void AddTitleCase(IConfigurator configurator, IServiceCollection services)
    {
        configurator
            .AddTagDataAction<TitleCaseAction>(services)
            .WithDescription("Convert all fields to title case.");
    }

    private static void AddFind(IConfigurator configurator, IServiceCollection services)
    {
        configurator
            .AddCommand<FindCommand>("find")
            .WithDescription("Find files by metadata")
            .WithExample("find", ".", "-q", "\"title | string.downcase |  string.contains 'dub'\"");
    }

    private static void AddTui(IConfigurator configurator, IServiceCollection services)
    {
        configurator.AddCommand<TuiApp>("ui").WithDescription("Interactive UI (TUI)");
    }
}
