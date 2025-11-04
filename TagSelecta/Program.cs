using Spectre.Console.Cli;
using TagSelecta.Actions;
using TagSelecta.BaseCommands;
using TagSelecta.DependencyInjection;

namespace TagSelecta;

class Program
{
    static int Main(string[] args)
    {
        bool noAnsi = Environment.GetEnvironmentVariable("TAGSELECTA_NOANSI") == "1";

        Spectre.Console.AnsiConsole.Profile.Capabilities.Ansi = !noAnsi;

        var app = new CommandApp(DependencyInjectionConfig.Configure());
        app.Configure(config =>
        {
            config
                .AddCommand<FileCommand<ReadAction, ReadSettings>>("read")
                .WithDescription("Read tags.");
            config
                .AddCommand<FileCommand<WriteAction, WriteSettings>>("write")
                .WithDescription("Write tags.")
                .WithExample(
                    ["write", "song.mp3", "-t", "Song1", "-a", "Artist1", "-a", "Artist2"]
                );
            config
                .AddCommand<FileCommand<CleanAction, CleanSettings>>("clean")
                .WithDescription("Cleans metadata, except the specified tags.")
                .WithExample(["clean", "song.mp3", "-e", "artist", "-e", "title"]);
            config
                .AddCommand<FileCommand<SplitAction, SplitSettings>>("split")
                .WithDescription("Split artists, album artists and composers");
            config
                .AddCommand<FileCommand<AutoTrackAction, AutoTrackSettings>>("autotrack")
                .WithDescription("Auto track.");
            config
                .AddCommand<FileCommand<RenameDirAction, RenameDirSettings>>("renamedir")
                .WithDescription("Rename directories.");
            config
                .AddCommand<FileCommand<RenameFileAction, RenameFileSettings>>("renamefile")
                .WithDescription("Rename files.");
            config
                .AddCommand<FileCommand<FixAlbumAction, FixAlbumSettings>>("fixalbum")
                .WithDescription(
                    "Set album name, year and album artists to the same value to all files in the same directory."
                );
            config
                .AddCommand<FileCommand<DiscogsAction, DiscogsSettings>>("discogs")
                .WithDescription(
                    "Update album from discogs. You can pass discogs release id (not master) or query to search."
                )
                .WithExample(["discogs", "path-to-album", "-r", "4202979"])
                .WithExample(["discogs", "song.mp3", "-r", "4202979", "-f", "picture"])
                .WithExample(["discogs", "path-to-album", "-q", "King Tubby Dub From The Roots"]);
            config
                .AddCommand<FileCommand<TitleCaseAction, TitleCaseSettings>>("titlecase")
                .WithDescription("Convert all field to title case.");
            config
                .AddCommand<FileCommand<VaAction, VaSettings>>("va")
                .WithDescription("Convert all field to title case.");
        });

        return app.Run(args);
    }
}
