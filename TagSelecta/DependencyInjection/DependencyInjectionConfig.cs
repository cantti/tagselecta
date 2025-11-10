using Microsoft.Extensions.DependencyInjection;
using TagSelecta.Actions.FileActions;
using TagSelecta.Actions.TagDataActions;
using TagSelecta.Configuration;
using TagSelecta.Discogs;

namespace TagSelecta.DependencyInjection;

public static class DependencyInjectionConfig
{
    public static TypeRegistrar Configure()
    {
        var services = new ServiceCollection();
        services.AddDiscogs();
        services.AddTransient<IConfig, Config>();
        services.AddTransient<ITagDataAction<DiscogsSettings>, DiscogsAction>();
        services.AddTransient<ITagDataAction<AutoTrackSettings>, AutoTrackAction>();
        services.AddTransient<ITagDataAction<SplitSettings>, SplitAction>();
        services.AddTransient<ITagDataAction<FixAlbumSettings>, FixAlbumAction>();
        services.AddTransient<ITagDataAction<TitleCaseSettings>, TitleCaseAction>();
        services.AddTransient<ITagDataAction<CleanSettings>, CleanAction>();
        services.AddTransient<ITagDataAction<VaSettings>, VaAction>();
        services.AddTransient<ITagDataAction<WriteSettings>, WriteAction>();
        services.AddTransient<IFileAction<RenameDirSettings>, RenameDirAction>();
        services.AddTransient<IFileAction<RenameFileSettings>, RenameFileAction>();
        services.AddTransient<IFileAction<ReadSettings>, ReadAction>();
        return new TypeRegistrar(services);
    }
}
