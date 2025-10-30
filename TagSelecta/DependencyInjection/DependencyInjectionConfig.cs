using Microsoft.Extensions.DependencyInjection;
using TagSelecta.Actions;
using TagSelecta.Actions.Base;
using TagSelecta.Infrastructure;

namespace TagSelecta.DependencyInjection;

public static class DependencyInjectionConfig
{
    private static void AddActions(IServiceCollection services)
    {
        services.AddTransient<IAction<ReadSettings>, ReadAction>();
        services.AddTransient<IAction<AutoTrackSettings>, AutoTrackAction>();
        services.AddTransient<IAction<CleanSettings>, CleanAction>();
        services.AddTransient<IAction<FixAlbumSettings>, FixAlbumAction>();
        services.AddTransient<IAction<RenameDirSettings>, RenameDirAction>();
        services.AddTransient<IAction<RenameFileSettings>, RenameFileAction>();
        services.AddTransient<IAction<WriteSettings>, WriteAction>();
    }

    public static TypeRegistrar Configure()
    {
        var registrations = new ServiceCollection();
        AddActions(registrations);
        return new TypeRegistrar(registrations);
    }
}
