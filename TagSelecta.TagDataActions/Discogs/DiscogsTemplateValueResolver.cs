using Scriban;
using Scriban.Runtime;
using TagSelecta.Shared.Exceptions;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Discogs.DiscogsApi;

namespace TagSelecta.TagDataActions.Discogs;

public static class DiscogsTemplateValueResolver
{
    public static IReadOnlyList<ReleaseTrack> GetTracks(Release release)
    {
        return release.TrackList
            ?.Where(x => string.Equals(x.Type, "track", StringComparison.OrdinalIgnoreCase))
            .ToList() ?? [];
    }

    public static string GetValue(string template, Release release, int index)
    {
        MemberRenamerDelegate memberRenamer = member => member.Name.ToLowerInvariant();

        var scriptObject = new ScriptObject();
        scriptObject.Import(typeof(DiscogsFunctions), renamer: memberRenamer);
        scriptObject.Import(
            new DiscogsTemplateModel
            {
                Release = release,
                Tracks = GetTracks(release),
                Index = index,
            },
            renamer: memberRenamer
        );

        var context = new TemplateContext();
        context.PushGlobal(scriptObject);

        var parsedTemplate = Template.Parse(template);
        if (parsedTemplate.HasErrors)
        {
            throw new TagSelectaException(parsedTemplate.Messages.ToString());
        }

        var result = parsedTemplate.Render(context);
        return result.Trim();
    }

    private sealed class DiscogsTemplateModel
    {
        public required Release Release { get; init; }
        public required IReadOnlyList<ReleaseTrack> Tracks { get; init; }
        public required int Index { get; init; }
    }
}

public static class DiscogsFunctions
{
    public static string Joined(object? input)
    {
        if (input is null)
        {
            return string.Empty;
        }

        if (input is string value)
        {
            return new[] { value }.ToJoined();
        }

        if (input is IEnumerable<object?> values)
        {
            return values.Select(x => x?.ToString()).ToJoined();
        }

        return new[] { input.ToString() }.ToJoined();
    }
}
