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
        services.AddTransient<TagDataAction<DiscogsSettings>, DiscogsAction>();
        services.AddTransient<TagDataAction<AutoTrackSettings>, AutoTrackAction>();
        services.AddTransient<TagDataAction<SplitSettings>, SplitAction>();
        services.AddTransient<TagDataAction<FixAlbumSettings>, FixAlbumAction>();
        services.AddTransient<TagDataAction<TitleCaseSettings>, TitleCaseAction>();
        services.AddTransient<TagDataAction<CleanSettings>, CleanAction>();
        services.AddTransient<TagDataAction<VaSettings>, VaAction>();
        services.AddTransient<TagDataAction<WriteSettings>, WriteAction>();
        services.AddTransient<FileAction<RenameDirSettings>, RenameDirAction>();
        services.AddTransient<FileAction<RenameFileSettings>, RenameFileAction>();
        services.AddTransient<FileAction<ReadSettings>, ReadAction>();
        return new TypeRegistrar(services);
    }
}
