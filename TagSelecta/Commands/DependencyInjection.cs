using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TagSelecta.BaseCommands;

namespace TagSelecta.Commands;

public static class DependencyInjection
{
    public static IServiceCollection AddCommands(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var fileCommandType = typeof(FileCommand<>);

        var implementations = assembly
            .GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .Where(t =>
                t.BaseType != null
                && t.BaseType.IsGenericType
                && t.BaseType.GetGenericTypeDefinition() == fileCommandType
            );

        foreach (var impl in implementations)
        {
            services.AddTransient(impl);
        }

        return services;
    }
}
