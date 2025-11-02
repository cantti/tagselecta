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
            config.AddCommand<FileCommand<ReadSettings>>("read").WithDescription("Read tags.");
            config
                .AddCommand<FileCommand<WriteSettings>>("write")
                .WithDescription("Write tags.")
                .WithExample(["write", "-t", "Song1", "-a", "Artist1", "-a", "Artist2"]);
            config
                .AddCommand<FileCommand<CleanSettings>>("clean")
                .WithDescription("Cleans metadata, except the specified tags.")
                .WithExample(["clean", "-e", "artist", "-e", "title"]);
            config
                .AddCommand<FileCommand<SplitSettings>>("split")
                .WithDescription("Split artists, album artists and composers");
            config
                .AddCommand<FileCommand<AutoTrackSettings>>("autotrack")
                .WithDescription("Auto track.");
            config
                .AddCommand<FileCommand<RenameDirSettings>>("renamedir")
                .WithDescription("Rename directories.");
            config
                .AddCommand<FileCommand<RenameFileSettings>>("renamefile")
                .WithDescription("Rename files.");
            config
                .AddCommand<FileCommand<FixAlbumSettings>>("fixalbum")
                .WithDescription(
                    "Set album name, year and album artists to the same value to all files in the same directory."
                );
            config
                .AddCommand<FileCommand<DiscogsSettings>>("discogs")
                .WithDescription("Update album from discogs release id.");
        });

        return app.Run(args);
    }
}
