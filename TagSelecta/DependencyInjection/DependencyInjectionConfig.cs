using Microsoft.Extensions.DependencyInjection;
using TagSelecta.Actions;
using TagSelecta.Actions.Base;
using TagSelecta.BaseCommands;
using TagSelecta.Discogs;
using TagSelecta.Print;

namespace TagSelecta.DependencyInjection;

public static class DependencyInjectionConfig
{
    private static void AddActions(IServiceCollection services)
    {
        services.AddTransient<Printer>();
        services.AddTransient(typeof(FileActionFactory<,>));
        services.AddDiscogs();
    }

    public static TypeRegistrar Configure()
    {
        var registrations = new ServiceCollection();
        AddActions(registrations);
        return new TypeRegistrar(registrations);
    }
}
