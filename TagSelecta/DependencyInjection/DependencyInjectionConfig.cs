using Microsoft.Extensions.DependencyInjection;
using TagSelecta.Discogs;

namespace TagSelecta.DependencyInjection;

public static class DependencyInjectionConfig
{
    public static TypeRegistrar Configure()
    {
        var services = new ServiceCollection();
        services.AddDiscogs();
        services.AddTransient<IConfig, Config>();
        return new TypeRegistrar(services);
    }
}
