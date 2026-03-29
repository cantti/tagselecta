using System.Text.RegularExpressions;
using TagLib;
using TagSelecta.Shared.Exceptions;
using TagSelecta.Shared.IO;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;
using TagSelecta.TagDataActions.Discogs.DiscogsApi;

namespace TagSelecta.TagDataActions.Discogs;

[TagDataActionName("discogs")]
public class DiscogsAction(
    IDiscogsApi discogsApi,
    DiscogsImageDownloader discogsImageDownloader,
    IAudioFileScanner fileScanner,
    DiscogsConfig discogsConfig
) : TagDataAction<DiscogsSettings>
{
    private Release? _release;
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
            var master = await discogsApi.GetMaster(urlId);
            releaseId = master.MainRelease;
        }

        _release = await discogsApi.GetRelease(releaseId);

        if (_release is null)
        {
            return false;
        }

        var imageUrl = _release.Images?.FirstOrDefault()?.Uri ?? string.Empty;
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
        var tracks = DiscogsTemplateValueResolver.GetTracks(_release);

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
            var value = DiscogsTemplateValueResolver.GetValue(entry.Value, _release, trackIndex);
            tagData.SetField(entry.FieldName, value);
        }

        tagData.Disc = "";
        tagData.DiscTotal = "";
        tagData.Picture = [new Picture(_releaseImage)];

        context.Target.UpdateTagData(tagData);
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
