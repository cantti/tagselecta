using Spectre.Console.Cli;
using TagSelecta.Actions;
using TagSelecta.BaseCommands;
using TagSelecta.DependencyInjection;

namespace TagSelecta;

class Program
{
    static int Main(string[] args)
    {
        var app = new CommandApp(DependencyInjectionConfig.Configure());
        app.Configure(config =>
        {
            config.AddCommand<FileCommand<ReadSettings>>("read").WithDescription("Read tags.");
            config.AddCommand<FileCommand<WriteSettings>>("write").WithDescription("Write tags.");
            config
                .AddCommand<FileCommand<CleanSettings>>("clean")
                .WithDescription("Remove unsupported tags.");
            config
                .AddCommand<FileCommand<FixAlbumSettings>>("fixalbum")
                .WithDescription(
                    "Set album name and album artists to the same value to all files in the same directory."
                );
            config
                .AddCommand<FileCommand<AutoTrackSettings>>("autotrack")
                .WithDescription("Auto track.");
            config
                .AddCommand<FileCommand<RenameDirSettings>>("renamedir")
                .WithDescription("Rename directories.");
            config
                .AddCommand<FileCommand<RenameFileSettings>>("renamefile")
                .WithDescription("Rename files.");
        });

        return app.Run(args);
    }
}
