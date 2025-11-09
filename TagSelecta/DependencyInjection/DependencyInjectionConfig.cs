using Microsoft.Extensions.DependencyInjection;
using TagSelecta.Discogs;
using TagSelecta.TagDataActions;

namespace TagSelecta.DependencyInjection;

public static class DependencyInjectionConfig
{
    public static TypeRegistrar Configure()
    {
        var services = new ServiceCollection();
        services.AddDiscogs();
        services.AddTransient<IConfig, Config>();
        services.AddTransient<DiscogsAction>();
        services.AddTransient<AutoTrackAction>();
        services.AddTransient<SplitAction>();
        services.AddTransient<FixAlbumAction>();
        services.AddTransient<TitleCaseAction>();
        services.AddTransient<CleanAction>();
        services.AddTransient<VaAction>();
        services.AddTransient<WriteAction>();
        return new TypeRegistrar(services);
    }
}
