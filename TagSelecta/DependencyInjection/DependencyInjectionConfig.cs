using Microsoft.Extensions.DependencyInjection;
using TagSelecta.Actions;
using TagSelecta.Actions.Base;
using TagSelecta.Discogs;
using TagSelecta.Print;

namespace TagSelecta.DependencyInjection;

public static class DependencyInjectionConfig
{
    private static void AddActions(IServiceCollection services)
    {
        services.AddTransient<FileAction<ReadSettings>, ReadAction>();
        services.AddTransient<FileAction<AutoTrackSettings>, AutoTrackAction>();
        services.AddTransient<FileAction<CleanSettings>, CleanAction>();
        services.AddTransient<FileAction<FixAlbumSettings>, FixAlbumAction>();
        services.AddTransient<FileAction<RenameDirSettings>, RenameDirAction>();
        services.AddTransient<FileAction<RenameFileSettings>, RenameFileAction>();
        services.AddTransient<FileAction<WriteSettings>, WriteAction>();
        services.AddTransient<FileAction<DiscogsSettings>, DiscogsAction>();

        services.AddTransient<Printer>();

        services.AddDiscogs();
    }

    public static TypeRegistrar Configure()
    {
        var registrations = new ServiceCollection();
        AddActions(registrations);
        return new TypeRegistrar(registrations);
    }
}
