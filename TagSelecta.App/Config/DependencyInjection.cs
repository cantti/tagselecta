using Microsoft.Extensions.DependencyInjection;
using TagSelecta.Commands.Tui;
using TagSelecta.Commands.Tui.TuiCommands;
using TagSelecta.TagDataActions.Discogs;
using TagSelecta.TagDataActions.MusicBrainz;

namespace TagSelecta.App.Config;

public static class DependencyInjection
{
    public static IServiceCollection AddSettings(
        this IServiceCollection services,
        ConfigModel configModel
    )
    {
        services.AddSingleton(new MacroConfig { Macros = configModel.Macros });
        services.AddSingleton(
            new TuiAppConfig
            {
                FileListRatio = configModel.General.FileListRatio,
                AutoCompletionEnabled = configModel.General.AutoCompletionEnabled,
                TreeEnabled = configModel.General.TreeEnabled,
            }
        );
        services.AddSingleton(CreateDiscogsConfig(configModel.Discogs));
        services.AddSingleton(CreateMusicBrainzConfig(configModel.MusicBrainz));
        return services;
    }

    private static DiscogsConfig CreateDiscogsConfig(DiscogsSection discogsSection)
    {
        var perTrackFields = new HashSet<string>(
            discogsSection.PerTrackFields,
            StringComparer.OrdinalIgnoreCase
        );

        return new DiscogsConfig
        {
            FieldMap = discogsSection
                .FieldMap.Select(x => new DiscogsFieldMapEntry(x.Key, x.Value, perTrackFields.Contains(x.Key)))
                .ToList(),
        };
    }

    private static MusicBrainzConfig CreateMusicBrainzConfig(MusicBrainzSection musicBrainzSection)
    {
        var perTrackFields = new HashSet<string>(
            musicBrainzSection.PerTrackFields,
            StringComparer.OrdinalIgnoreCase
        );

        return new MusicBrainzConfig
        {
            FieldMap = musicBrainzSection
                .FieldMap.Select(x =>
                    new MusicBrainzFieldMapEntry(x.Key, x.Value, perTrackFields.Contains(x.Key))
                )
                .ToList(),
        };
    }
}
