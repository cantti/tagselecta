using Microsoft.Extensions.DependencyInjection;
using Refit;
using TagSelecta.Actions;
using TagSelecta.Actions.Base;
using TagSelecta.Discogs;
using TagSelecta.Print;

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
        services.AddTransient<FileAction<DiscogsSettings>, DiscogsAction>();

        services
            .AddRefitClient<IDiscogsApi>(
                new()
                {
                    AuthorizationHeaderValueGetter = (_, _) =>
                    {
                        return Task.FromResult(
                            "key=irBlmropPHHUtaZceGyW, secret=rmnYnuNKxHLxTfVMuJJAjtRRhOuBPQmS"
                        );
                    },
                }
            )
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.discogs.com"));

        services.AddTransient<Printer>();
    }

    public static TypeRegistrar Configure()
    {
        var registrations = new ServiceCollection();
        AddActions(registrations);
        return new TypeRegistrar(registrations);
    }
}
