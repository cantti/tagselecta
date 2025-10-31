using Microsoft.Extensions.DependencyInjection;
using TagSelecta.Actions;
using TagSelecta.Actions.Base;

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
    }

    public static TypeRegistrar Configure()
    {
        var registrations = new ServiceCollection();
        AddActions(registrations);
        return new TypeRegistrar(registrations);
    }
}
