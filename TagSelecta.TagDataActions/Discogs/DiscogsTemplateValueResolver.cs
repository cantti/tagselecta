using System.Collections;
using Scriban;
using Scriban.Runtime;
using TagSelecta.Shared.Exceptions;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Discogs.DiscogsApi.ReleaseModels;

namespace TagSelecta.TagDataActions.Discogs;

public static class DiscogsTemplateValueResolver
{
    public static string GetValue(string template, Release release, int index)
    {
        MemberRenamerDelegate memberRenamer = member => member.Name.ToLowerInvariant();

        var scriptObject = new ScriptObject();
        scriptObject.Import(typeof(DiscogsFunctions));
        scriptObject.Import(
            new DiscogsTemplateModel
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

    public static List<Track> GetTracks(Release release)
    {
        return release
                .TrackList?.Where(x =>
                    string.Equals(x.Type, "track", StringComparison.OrdinalIgnoreCase)
                )
                .ToList() ?? [];
    }

    private sealed class DiscogsTemplateModel
    {
        public required Release Release { get; init; }
        public required List<Track> Tracks { get; init; }
        public required int Index { get; init; }
    }

    private static class DiscogsFunctions
    {
        public static string? Joined(IEnumerable? input)
        {
            var list = input?.Cast<string?>() ?? [];
            return list.JoinTagValues();
        }
    }
}
