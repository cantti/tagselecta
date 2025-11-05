using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TagSelecta.BaseCommands;
using TagSelecta.Discogs;

namespace TagSelecta.DependencyInjection;

public static class DependencyInjectionConfig
{
    public static TypeRegistrar Configure()
    {
        var services = new ServiceCollection();
        services.AddDiscogs();
        return new TypeRegistrar(services);
    }
}
