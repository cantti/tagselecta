using System.Collections;
using Scriban;
using Scriban.Runtime;
using TagSelecta.Shared.Exceptions;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

namespace TagSelecta.TagDataActions.MusicBrainz;

public static class MusicBrainzTemplateValueResolver
{
    public static List<TrackTemplateModel> GetTracks(Release release)
    {
        var mediaList = release.Media ?? [];
        var discTotal = mediaList.Count;
        var tracks = new List<TrackTemplateModel>();

        for (var mediaIndex = 0; mediaIndex < mediaList.Count; mediaIndex++)
        {
            var media = mediaList[mediaIndex];
            var mediaTracks = media.Tracks ?? [];
            var discNumber = media.Position is > 0 ? media.Position.Value : mediaIndex + 1;
            var trackTotal = media.TrackCount is > 0 ? media.TrackCount.Value : mediaTracks.Count;

            for (var trackIndex = 0; trackIndex < mediaTracks.Count; trackIndex++)
            {
                var track = mediaTracks[trackIndex];
                var trackNumber =
                    track.Position is > 0
                        ? track.Position.Value
                        : int.TryParse(track.Number, out var parsedNumber) && parsedNumber > 0
                            ? parsedNumber
                            : trackIndex + 1;

                tracks.Add(
                    new TrackTemplateModel
                    {
                        Id = track.Id,
                        Title = track.Title,
                        Length = track.Length,
                        Number = track.Number,
                        Position = track.Position,
                        ArtistCredit = track.ArtistCredit,
                        Recording = track.Recording,
                        DiscNumber = discNumber,
                        DiscTotal = discTotal,
                        TrackNumber = trackNumber,
                        TrackTotal = trackTotal,
                    }
                );
            }
        }

        return tracks;
    }

    public static string GetValue(string template, Release release, int index)
    {
        MemberRenamerDelegate memberRenamer = member => member.Name.ToLowerInvariant();

        var scriptObject = new ScriptObject();
        scriptObject.Import(typeof(MusicBrainzFunctions));
        scriptObject.Import(
            new MusicBrainzTemplateModel
            {
                Release = release,
                Tracks = GetTracks(release),
                Index = index,
            }
        );

        var context = new TemplateContext { MemberRenamer = memberRenamer };
        context.PushGlobal(scriptObject);

        var parsedTemplate = Template.Parse(template);
        if (parsedTemplate.HasErrors)
        {
            throw new TagSelectaException(parsedTemplate.Messages.ToString());
        }

        var result = parsedTemplate.Render(context);
        return result.Trim();
    }

    private sealed class MusicBrainzTemplateModel
    {
        public required Release Release { get; init; }
        public required List<TrackTemplateModel> Tracks { get; init; }
        public required int Index { get; init; }
    }

    public sealed class TrackTemplateModel
    {
        public string? Id { get; init; }
        public string? Title { get; init; }
        public int? Length { get; init; }
        public string? Number { get; init; }
        public int? Position { get; init; }
        public List<ArtistCredit>? ArtistCredit { get; init; }
        public Recording? Recording { get; init; }
        public int DiscNumber { get; init; }
        public int DiscTotal { get; init; }
        public int TrackNumber { get; init; }
        public int TrackTotal { get; init; }
    }

    private static class MusicBrainzFunctions
    {
        public static string? Joined(IEnumerable? input)
        {
            var list = input?.Cast<string?>() ?? [];
            return list.JoinTagValues();
        }
    }
}
