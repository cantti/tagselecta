using Microsoft.Extensions.DependencyInjection;
using TagSelecta.Shared.IO;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.Shared;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedServices(this IServiceCollection services)
    {
        services.AddTransient<IAudioFileScanner, AudioFileScanner>();
        services.AddTransient<ITagger, Tagger>();
        services.AddTransient<IFileSystem, FileSystem>();
        return services;
    }
}
