using System.Text.Json;
using TagSelecta.Shared.Exceptions;
using TagSelecta.TagDataActions.Abstractions;
using TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

namespace TagSelecta.TagDataActions.MusicBrainz;

[TagDataActionName("musicbrainz", "mb")]
public class MusicBrainzAction(IMusicBrainzApiClient musicBrainzApiClient)
    : TagDataAction<MusicBrainzSettings>
{
    private static readonly (string TagField, string JsonPath)[] FieldMap =
    [
        ("album", "title"),
        ("albumartist", "artist-credit[0].artist.name"),
        ("artist", "artist-credit[0].artist.name"),
        ("date", "date"),
        ("label", "label-info[0].label.name"),
        ("catalognumber", "label-info[0].catalog-number"),
        ("comment", "disambiguation"),
    ];

    private JsonElement? _release;

    public override async Task<bool> BeforeExecuteAsync(
        MusicBrainzSettings settings,
        CancellationToken token
    )
    {
        _release = await musicBrainzApiClient.GetRelease(
            "edf5b60c-4888-4faf-9d6c-a204b84d4e79",
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

        foreach (var (tagField, jsonPath) in FieldMap)
        {
            tagData.SetField(tagField, GetReleaseValue(_release.Value, jsonPath));
        }

        context.Target.UpdateTagData(tagData);
    }

    private static string GetReleaseValue(JsonElement release, string path)
    {
        var value = ResolvePathValue(release, path);
        return value is null ? string.Empty : JsonElementToString(value.Value);
    }

    private static JsonElement? ResolvePathValue(JsonElement current, string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        var segments = path.Split('.', StringSplitOptions.RemoveEmptyEntries);

        foreach (var rawSegment in segments)
        {
            var segment = rawSegment.Trim();
            var bracketStart = segment.IndexOf('[');
            var propertyName = bracketStart >= 0 ? segment[..bracketStart] : segment;

            if (!string.IsNullOrWhiteSpace(propertyName))
            {
                if (!TryGetPropertyCaseInsensitive(current, propertyName, out current))
                {
                    return null;
                }
            }

            if (bracketStart >= 0)
            {
                var bracketEnd = segment.IndexOf(']', bracketStart + 1);

                if (bracketEnd < 0 || bracketEnd != segment.Length - 1)
                {
                    return null;
                }

                var indexText = segment.Substring(bracketStart + 1, bracketEnd - bracketStart - 1);

                if (!int.TryParse(indexText, out var index))
                {
                    return null;
                }

                if (!TryGetArrayElement(current, index, out current))
                {
                    return null;
                }
            }
        }

        return current;
    }

    private static bool TryGetPropertyCaseInsensitive(
        JsonElement element,
        string propertyName,
        out JsonElement value
    )
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            value = default;
            return false;
        }

        if (element.TryGetProperty(propertyName, out value))
        {
            return true;
        }

        foreach (var property in element.EnumerateObject())
        {
            if (property.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            {
                value = property.Value;
                return true;
            }
        }

        value = default;
        return false;
    }

    private static bool TryGetArrayElement(JsonElement element, int index, out JsonElement value)
    {
        if (element.ValueKind != JsonValueKind.Array || index < 0)
        {
            value = default;
            return false;
        }

        var i = 0;
        foreach (var item in element.EnumerateArray())
        {
            if (i == index)
            {
                value = item;
                return true;
            }

            i++;
        }

        value = default;
        return false;
    }

    private static string JsonElementToString(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString() ?? string.Empty,
            JsonValueKind.Null => string.Empty,
            JsonValueKind.Undefined => string.Empty,
            JsonValueKind.Array => string.Join(
                "; ",
                element
                    .EnumerateArray()
                    .Select(JsonElementToString)
                    .Where(static x => !string.IsNullOrWhiteSpace(x))
            ),
            _ => element.GetRawText(),
        };
    }
}
