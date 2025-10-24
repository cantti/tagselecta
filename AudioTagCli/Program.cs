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
            config.AddCommand<FixAlbumArtistCommand>("fixalbumartist");
            config.AddCommand<CleanCommand>("clean");
            config.AddCommand<ReadCommand>("read");
            config.AddCommand<WriteCommand>("write");
        });

        return app.Run(args);
    }
}
