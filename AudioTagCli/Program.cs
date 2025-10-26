using AudioTagCli.Commands;
using Spectre.Console.Cli;

namespace AudioTagCli;

class Program
{
    static int Main(string[] args)
    {
        var app = new CommandApp();

        app.Configure(config =>
        {
            config.AddCommand<ReadCommand>("read").WithDescription("Read tags.");
            config.AddCommand<WriteCommand>("write").WithDescription("Write tags.");
            config.AddCommand<CleanCommand>("clean").WithDescription("Remove unsupported tags.");
            config
                .AddCommand<FixAlbumCommand>("fixalbum")
                .WithDescription(
                    "Set album name and album artists to the same value to all files in the same directory."
                );
            config.AddCommand<AutoTrackCommand>("autotrack").WithDescription("Auto track.");
        });

        return app.Run(args);
    }
}
