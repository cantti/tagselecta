namespace TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

public interface IMusicBrainzApiClient
{
    Task<string?> GetRelease(string id, CancellationToken token);
}
