using Spectre.Console.Cli;
using TagSelecta.Cli.DependencyInjection;
using TagSelecta.Commands;
using TagSelecta.Commands.FileCommands;
using TagSelecta.Commands.TagDataCommands;

namespace TagSelecta.Cli;

class Program
{
    static int Main(string[] args)
    {
        bool noAnsi = Environment.GetEnvironmentVariable("TAGSELECTA_NOANSI") == "1";

        Spectre.Console.AnsiConsole.Profile.Capabilities.Ansi = !noAnsi;

        var app = new CommandApp(new TypeRegistrar(Commands.DependencyInjection.Configure()));
        app.Configure(config =>
        {
            config.AddCommand<FileCommand<ReadSettings>>("read").WithDescription("Read tags.");
            config
                .AddCommand<TagDataCommand<WriteSettings>>("write")
                .WithDescription("Write tags.")
                .WithExample(
                    ["write", "song.mp3", "-t", "Song1", "-a", "Artist1", "-a", "Artist2"]
                );
            config
                .AddCommand<TagDataCommand<CleanSettings>>("clean")
                .WithDescription("Cleans metadata, except the specified tags.")
                .WithExample(["clean", "song.mp3", "-e", "artist", "-e", "title"]);
            config
                .AddCommand<TagDataCommand<SplitSettings>>("split")
                .WithDescription("Split artists, album artists and composers");
            config
                .AddCommand<TagDataCommand<AutoTrackSettings>>("autotrack")
                .WithDescription("Auto track.");
            config
                .AddCommand<FileCommand<RenameDirSettings>>("renamedir")
                .WithDescription("Rename directories.");
            config
                .AddCommand<FileCommand<RenameFileSettings>>("renamefile")
                .WithDescription("Rename files.");
            config
                .AddCommand<TagDataCommand<FixAlbumSettings>>("fixalbum")
                .WithDescription(
                    "Set album name, year and album artists to the same value to all files in the same directory."
                );
            config
                .AddCommand<TagDataCommand<DiscogsSettings>>("discogs")
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
            config
                .AddCommand<TagDataCommand<TitleCaseSettings>>("titlecase")
                .WithDescription("Convert all field to title case.");
            config
                .AddCommand<TagDataCommand<VaSettings>>("va")
                .WithDescription("Normalize Various Artists values");
            config
                .AddCommand<FindCommand>("find")
                .WithDescription("Find files by metadata")
                .WithExample(
                    ["find", ".", "-q", "\"title | string.downcase |  string.contains 'dub'\""]
                );
        });

        return app.Run(args);
    }
}
