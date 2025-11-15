using Spectre.Console.Cli;
using TagSelecta.Cli.Commands;
using TagSelecta.Cli.Commands.FileCommands;
using TagSelecta.Cli.Commands.TagDataCommands;

namespace TagSelecta.Cli;

class Program
{
    static int Main(string[] args)
    {
        bool noAnsi = Environment.GetEnvironmentVariable("TAGSELECTA_NOANSI") == "1";

        Spectre.Console.AnsiConsole.Profile.Capabilities.Ansi = !noAnsi;

        var app = new CommandApp(new TypeRegistrar(DependencyInjection.Configure()));
        app.Configure(config =>
        {
            config.AddCommand<FileCommand<ReadSettings>>("read").WithDescription("Read tags.");
            config
                .AddCommand<TagDataCommand<WriteSettings>>("write")
                .WithDescription("Write tags.")
                // Basic examples
                .WithExample(["write", "song.mp3", "-t", "'Song 1'", "-a", "'Artist1;Artist 2'"])
                .WithExample(["write", "song.mp3", "-c", "'url=https://github.com'"])
                // Title, album, year
                .WithExample(
                    ["write", "song.mp3", "-t", "'My Track'", "-l", "'Best Album'", "-y", "2024"]
                )
                // Genre with multiple values
                .WithExample(["write", "song.mp3", "-g", "'Rock;Heavy Metal;Punk'"])
                // Artist, album artist, label
                .WithExample(
                    [
                        "write",
                        "song.mp3",
                        "-a",
                        "'John Doe'",
                        "-A",
                        "'Various Artists'",
                        "--label",
                        "'Example Records'",
                    ]
                )
                // Track and disc info
                .WithExample(["write", "song.mp3", "-n", "5", "-N", "12", "-d", "1", "-D", "2"])
                // Multiple composers
                .WithExample(
                    ["write", "song.mp3", "--composers", "'Composer 1;Composer 2;Composer 3'"]
                )
                // Custom tags with multiple key=value pairs
                .WithExample(
                    ["write", "song.mp3", "-c", "'key1=value1;key2=Some Value;key3=Another Value'"]
                )
                .WithExample(
                    [
                        "write",
                        "song.mp3",
                        "-a",
                        "'{{ artist | regex.replace \"^VA$\" \"Various Artists\" \"-i\" }}'",
                    ]
                );
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
                .WithDescription("Convert all fields to title case.");
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
