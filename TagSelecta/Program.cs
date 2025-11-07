using Spectre.Console.Cli;
using TagSelecta.Commands;
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
            config.AddCommand<ReadCommand>("read").WithDescription("Read tags.");
            config
                .AddCommand<WriteCommand>("write")
                .WithDescription("Write tags.")
                .WithExample(
                    ["write", "song.mp3", "-t", "Song1", "-a", "Artist1", "-a", "Artist2"]
                );
            config
                .AddCommand<CleanCommand>("clean")
                .WithDescription("Cleans metadata, except the specified tags.")
                .WithExample(["clean", "song.mp3", "-e", "artist", "-e", "title"]);
            config
                .AddCommand<SplitCommand>("split")
                .WithDescription("Split artists, album artists and composers");
            config.AddCommand<AutoTrackCommand>("autotrack").WithDescription("Auto track.");
            config.AddCommand<RenameDirCommand>("renamedir").WithDescription("Rename directories.");
            config.AddCommand<RenameFileCommand>("renamefile").WithDescription("Rename files.");
            config
                .AddCommand<FixAlbumCommand>("fixalbum")
                .WithDescription(
                    "Set album name, year and album artists to the same value to all files in the same directory."
                );
            config
                .AddCommand<DiscogsCommand>("discogs")
                .WithDescription(
                    "Update album from discogs. You can pass discogs release id (not master) or query to search."
                )
                .WithExample(
                    [
                        "discogs",
                        "path-to-album",
                        "-u",
                        "https://www.discogs.com/release/4202979-King-Tubby-Dub-From-The-Roots",
                    ]
                )
                .WithExample(["discogs", "path-to-album", "-q", "King Tubby Dub From The Roots"]);
            config
                .AddCommand<TitleCaseCommand>("titlecase")
                .WithDescription("Convert all field to title case.");
            config.AddCommand<VaCommand>("va").WithDescription("Normalize Various Artists values");
            config
                .AddCommand<FindCommand>("find")
                .WithDescription("Find files by metadata")
                .WithExample(
                    ["find", ".", "-q", "title | string.downcase |  string.contains 'dub'"]
                );
        });

        return app.Run(args);
    }
}
