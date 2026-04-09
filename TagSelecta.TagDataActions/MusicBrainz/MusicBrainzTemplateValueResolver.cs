using Scriban;
using Scriban.Runtime;
using TagSelecta.Shared.Exceptions;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.MusicBrainz.MusicBrainzApi;

namespace TagSelecta.TagDataActions.MusicBrainz;

public static class MusicBrainzTemplateValueResolver
{
    public static List<Track> GetTracks(Release release)
    {
        return release.Media?.SelectMany(x => x.Tracks ?? []).ToList() ?? [];
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
        public required List<Track> Tracks { get; init; }
        public required int Index { get; init; }
    }

    private static class MusicBrainzFunctions
    {
        public static string Joined(IEnumerable<object> input)
        {
            return input.Select(x => x.ToString()).JoinTagValues();
        }
    }
}
