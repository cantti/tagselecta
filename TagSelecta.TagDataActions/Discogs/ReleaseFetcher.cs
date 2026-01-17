using System.Text.RegularExpressions;
using TagSelecta.Shared.Exceptions;
using TagSelecta.TagDataActions.Discogs.DiscogsApi;

namespace TagSelecta.TagDataActions.Discogs;

public class ReleaseFetcher(IDiscogsApi discogsApi, DiscogsImageDownloader discogsImageDownloader)
    : IReleaseFetcher
{
    public async Task<ReleaseFetcherResult?> Fetch(DiscogsSettings settings)
    {
        var (urlType, urlId) = GetDiscogsReleaseInfo(settings.Url);
        var releaseId =
            urlType == "master" ? (await discogsApi.GetMaster(urlId)).MainRelease : urlId;
        var result = await discogsApi.GetRelease(releaseId);
        result.TrackList = result.TrackList.Where(x => x.Type == "track").ToList();
        var image = result.Images.FirstOrDefault();
        byte[]? resultImage = null;
        if (image is not null)
        {
            var bytes = await discogsImageDownloader.DownloadAsync(image.Uri);
            resultImage = bytes;
        }
        return new ReleaseFetcherResult { Release = result, Image = resultImage };
    }

    private static (string Type, int Id) GetDiscogsReleaseInfo(string input)
    {
        string pattern = @"/(release|master)/(\d+)";
        var match = Regex.Match(input, pattern);
        return match.Success
            ? (match.Groups[1].Value, int.Parse(match.Groups[2].Value))
            : throw new TagSelectaException("Error parsing discogs url");
    }
}

public class ReleaseFetcherResult
{
    public required Release Release { get; set; }
    public required byte[]? Image { get; set; }
}
