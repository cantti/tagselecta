using System.Text.RegularExpressions;
using System.Text.Json.Nodes;
using TagLib;
using TagSelecta.Shared.Exceptions;
using TagSelecta.Shared.IO;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;
using TagSelecta.TagDataActions.Discogs.DiscogsApi;
using TagSelecta.TagDataActions.Json;

namespace TagSelecta.TagDataActions.Discogs;

[TagDataActionName("discogs")]
public class DiscogsAction(
    IDiscogsApi discogsApi,
    DiscogsImageDownloader discogsImageDownloader,
    IAudioFileScanner fileScanner,
    DiscogsConfig discogsConfig
) : TagDataAction<DiscogsSettings>
{
    private string? _release;
    private byte[]? _releaseImage;

    public override async Task<bool> BeforeExecuteAsync(
        DiscogsSettings settings,
        CancellationToken token
    )
    {
        var (urlType, urlId) = GetDiscogsReleaseInfo(settings.Url);
        var releaseId = urlId;
        if (urlType == "master")
        {
            var master = await discogsApi.GetMaster(urlId, token);
            if (master is null)
            {
                return false;
            }

            releaseId = GetMainReleaseId(master);
        }

        _release = await discogsApi.GetRelease(releaseId, token);

        if (_release is null)
        {
            return false;
        }

        var imageUrl = JsonPathValueResolver.GetValue(_release, "$.images[0].uri");
        if (!string.IsNullOrWhiteSpace(imageUrl))
        {
            _releaseImage = await discogsImageDownloader.DownloadAsync(imageUrl);
        }

        return true;
    }

    protected override void Execute(TagDataActionExecuteContext<DiscogsSettings> context)
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

        var tracks = JsonPathValueResolver.GetNodes(_release, "$.tracklist[?(@.type_=='track')]");

        if (trackIndex > tracks.Count - 1)
        {
            return;
        }

        if (trackIndex < 0)
        {
            return;
        }

        foreach (var entry in discogsConfig.FieldMap)
        {
            var value = GetMappedValue(entry, _release, trackIndex);
            tagData.SetField(entry.FieldName, value);
        }

        tagData.TrackTotal = tracks.Count.ToString();
        tagData.Disc = "";
        tagData.DiscTotal = "";
        tagData.Picture = [new Picture(_releaseImage)];

        context.Target.UpdateTagData(tagData);
    }

    private static string GetMappedValue(DiscogsFieldMapEntry entry, string release, int trackIndex)
    {
        if (entry.Value.Equals("auto", StringComparison.OrdinalIgnoreCase))
        {
            return GetAutoValue(entry.FieldName, release, trackIndex);
        }

        return entry.PerTrack
            ? JsonPathValueResolver.GetIndexedValue(release, entry.Value, trackIndex)
            : JsonPathValueResolver.GetValue(release, entry.Value);
    }

    private static string GetAutoValue(string fieldName, string release, int trackIndex)
    {
        return fieldName.ToLowerInvariant() switch
        {
            "albumartist" => string.Join("; ", GetArtists(release)),
            "artist" => string.Join(
                "; ",
                GetTrackArtists(release, trackIndex) is { Count: > 0 } trackArtists
                    ? trackArtists
                    : GetArtists(release)
            ),
            "track" => trackIndex >= 0 ? (trackIndex + 1).ToString() : string.Empty,
            _ => string.Empty,
        };
    }

    private static List<string> GetTrackArtists(string release, int trackIndex)
    {
        if (trackIndex < 0)
        {
            return [];
        }

        var tracks = JsonPathValueResolver.GetNodes(release, "$.tracklist[?(@.type_=='track')]");
        if (trackIndex >= tracks.Count)
        {
            return [];
        }

        return tracks[trackIndex] is null ? [] : GetArtists(tracks[trackIndex]!.ToJsonString());
    }

    private static List<string> GetArtists(string token)
    {
        return JsonPathValueResolver
            .GetValues(token, "$.artists[*].name")
            .Select(RemoveTrailingNumberParentheses)
            .Where(static x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x!)
            .ToList();
    }

    private static int GetMainReleaseId(string master)
    {
        var value = JsonPathValueResolver.GetValue(master, "$.main_release");
        if (!int.TryParse(value, out var releaseId))
        {
            throw new TagSelectaException("Master release id not found");
        }

        return releaseId;
    }

    private static string? RemoveTrailingNumberParentheses(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        var result = Regex.Replace(input, @"\s*\(\d+\)\s*$", "");
        return result.TrimEnd();
    }

    private static (string Type, int Id) GetDiscogsReleaseInfo(string input)
    {
        var pattern = @"/(release|master)/(\d+)";
        var match = Regex.Match(input, pattern);
        return match.Success
            ? (match.Groups[1].Value, int.Parse(match.Groups[2].Value))
            : throw new TagSelectaException("Error parsing discogs url");
    }
}
