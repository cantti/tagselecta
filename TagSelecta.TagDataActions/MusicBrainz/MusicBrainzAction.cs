using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TagSelecta.Shared.Exceptions;
using TagSelecta.Shared.IO;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;
using TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

namespace TagSelecta.TagDataActions.MusicBrainz;

[TagDataActionName("musicbrainz", "mb")]
public class MusicBrainzAction(IMusicBrainzApiClient musicBrainzApiClient, IAudioFileScanner fileScanner)
    : TagDataAction<MusicBrainzSettings>
{
    private static readonly (string TagField, string JsonPath)[] FieldMap =
    [
        ("album", "$['title']"),
        ("albumartist", "$['artist-credit'][0]['artist']['name']"),
        ("artist", "$['artist-credit'][0]['artist']['name']"),
        ("title", "$['media'][*]['tracks'][*]['title']"),
        ("genre", "$['release-group']['genres'][*]['name']"),
        ("date", "$['date']"),
        ("label", "$['label-info'][0]['label']['name']"),
        ("catalognumber", "$['label-info'][0]['catalog-number']"),
        ("comment", "$['disambiguation']"),
        ("musicbrainz_release_id", "$['id']"),
        ("musicbrainz_country", "$['country']"),
        ("musicbrainz_status", "$['status']"),
        ("musicbrainz_quality", "$['quality']"),
        ("musicbrainz_barcode", "$['barcode']"),
        ("musicbrainz_asin", "$['asin']"),
        ("musicbrainz_packaging", "$['packaging']"),
        ("musicbrainz_language", "$['text-representation']['language']"),
        ("musicbrainz_script", "$['text-representation']['script']"),
        ("musicbrainz_coverart_count", "$['cover-art-archive']['count']"),
        ("musicbrainz_coverart_front", "$['cover-art-archive']['front']"),
        ("musicbrainz_coverart_back", "$['cover-art-archive']['back']"),
    ];

    private JToken? _release;

    public override async Task<bool> BeforeExecuteAsync(
        MusicBrainzSettings settings,
        CancellationToken token
    )
    {
        _release = await musicBrainzApiClient.GetRelease(
            "10f5fd34-470d-46f8-b364-7e2ddfc765e5",
            token
        );
        return _release is not null;
    }

    protected override void Execute(TagDataActionExecuteContext<MusicBrainzSettings> context)
    {
        if (_release is null)
        {
            throw new TagSelectaException("Release not set");
        }

        var tagData = context.Target.CurrentTagData;

        var directoryFiles = fileScanner
            .Search([context.Target.BackupPath.DirectoryName()])
            .Order()
            .ToList();

        var trackIndex = directoryFiles.FindIndex(x => x == context.Target.BackupPath);

        foreach (var (tagField, jsonPath) in FieldMap)
        {
            if (tagField == "title")
            {
                var titles = _release!
                    .SelectTokens(jsonPath, false)
                    .Select(JTokenToString)
                    .Where(static x => !string.IsNullOrWhiteSpace(x))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                tagData.SetField(
                    tagField,
                    trackIndex >= 0 && trackIndex < titles.Count ? titles[trackIndex] : string.Empty
                );
                continue;
            }

            tagData.SetField(tagField, GetReleaseValue(_release!, jsonPath));
        }

        context.Target.UpdateTagData(tagData);
    }

    private static string GetReleaseValue(JToken release, string path)
    {
        return string.Join(
            "; ",
            release
                .SelectTokens(path, false)
                .Select(JTokenToString)
                .Where(static x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
        );
    }

    private static string JTokenToString(JToken? token)
    {
        var valueToken = token;
        if (valueToken is null)
        {
            return string.Empty;
        }

        return valueToken.Type switch
        {
            JTokenType.Null => string.Empty,
            JTokenType.Undefined => string.Empty,
            JTokenType.String => valueToken.Value<string>() ?? string.Empty,
            JTokenType.Array => string.Join(
                "; ",
                valueToken
                    .Children()
                    .Select(JTokenToString)
                    .Where(static x => !string.IsNullOrWhiteSpace(x))
            ),
            _ => valueToken.ToString(Formatting.None),
        };
    }
}
