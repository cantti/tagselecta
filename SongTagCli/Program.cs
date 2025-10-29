using SongTagCli.Actions;
using SongTagCli.BaseCommands;
using Spectre.Console.Cli;

namespace SongTagCli;

class Program
{
    static int Main(string[] args)
    {
        var app = new CommandApp();

        app.Configure(config =>
        {
            config
                .AddCommand<FileCommand<ReadSettings, ReadAction>>("read")
                .WithDescription("Read tags.");
            config
                .AddCommand<FileCommand<WriteSettings, WriteAction>>("write")
                .WithDescription("Write tags.");
            config
                .AddCommand<FileCommand<CleanSettings, CleanAction>>("clean")
                .WithDescription("Remove unsupported tags.");
            config
                .AddCommand<FileCommand<FixAlbumSettings, FixAlbumAction>>("fixalbum")
                .WithDescription(
                    "Set album name and album artists to the same value to all files in the same directory."
                );
            config
                .AddCommand<FileCommand<AutoTrackSettings, AutoTrackAction>>("autotrack")
                .WithDescription("Auto track.");
            config
                .AddCommand<FileCommand<RenameDirSettings, RenameDirAction>>("renamedir")
                .WithDescription("Rename directories.");
            config
                .AddCommand<FileCommand<RenameFileSettings, RenameFileAction>>("renamefile")
                .WithDescription("Rename files.");
        });

        return app.Run(args);
    }
}
