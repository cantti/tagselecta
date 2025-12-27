using Microsoft.Extensions.DependencyInjection;
using TagSelecta.Cli.Commands.AutoTrack;
using TagSelecta.Cli.Commands.Common;
using TagSelecta.Cli.Commands.Discogs;
using TagSelecta.Cli.Commands.ExtractPicture;
using TagSelecta.Cli.Commands.FileCommands;
using TagSelecta.Cli.Commands.FixAlbum;
using TagSelecta.Cli.Commands.Split;
using TagSelecta.Cli.Commands.TitleCase;
using TagSelecta.Cli.Commands.Write;
using TagSelecta.Cli.Discogs;
using TagSelecta.Cli.IO;
using TagSelecta.Shared.Configuration;
using TagSelecta.Tagging;

namespace TagSelecta.Cli;

public static class DependencyInjection
{
    public static IServiceCollection Configure()
    {
        var services = new ServiceCollection();
        services.AddDiscogs();
        services.AddTransient<IConfig, Config>();
        services.AddTransient<IFileSystem, FileSystem>();
        services.AddTransient<ITagger, Tagger>();
        services.AddTransient<IAudioFileScanner, AudioFileScanner>();
        services.AddTransient<TagDataAction<DiscogsSettings>, DiscogsAction>();
        services.AddTransient<TagDataAction<AutoTrackSettings>, AutoTrackAction>();
        services.AddTransient<TagDataAction<SplitSettings>, SplitAction>();
        services.AddTransient<TagDataAction<FixAlbumSettings>, FixAlbumAction>();
        services.AddTransient<TagDataAction<TitleCaseSettings>, TitleCaseAction>();
        services.AddTransient<TagDataAction<WriteSettings>, WriteAction>();
        services.AddTransient<FileAction<RenameDirSettings>, RenameDirAction>();
        services.AddTransient<TagDataAction<ExtractPictureSettings>, ExtractPictureAction>();
        return services;
    }
}
