using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace TagSelecta.TagDataActions.Json;

public static class JsonPathValueResolver
{
    public static List<JsonNode?> GetNodes(string json, string path)
    {
        return Evaluate(json, path).ToList();
    }

    public static int Count(string json, string path)
    {
        return Evaluate(json, path).Count;
    }

    public static string GetValue(string json, string path)
    {
        return string.Join("; ", GetValues(json, path));
    }

    public static string GetIndexedValue(string json, string path, int index)
    {
        if (index < 0)
        {
            return string.Empty;
        }

        if (HasIndexTemplate(path))
        {
            var resolvedPath = ResolveIndexTemplate(path, index);
            return GetValue(json, resolvedPath);
        }

        var values = Evaluate(json, path)
            .Select(ToTagString)
            .Where(static x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        return index < values.Count ? values[index] : string.Empty;
    }

    public static List<string> GetValues(string json, string path)
    {
        return Evaluate(json, path)
            .Select(ToTagString)
            .Where(static x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static IReadOnlyList<JsonNode?> Evaluate(string json, string path)
    {
        return JqWrapper.Evaluate(json, path);
    }

    private static bool HasIndexTemplate(string path)
    {
        return Regex.IsMatch(path, "\\{\\{\\s*index\\s*\\}\\}", RegexOptions.IgnoreCase);
    }

    private static string ResolveIndexTemplate(string path, int index)
    {
        return Regex.Replace(
            path,
            "\\{\\{\\s*index\\s*\\}\\}",
            index.ToString(),
            RegexOptions.IgnoreCase
        );
    }

    public static string ToTagString(JsonNode? token)
    {
        if (token is null)
        {
            return string.Empty;
        }

        return token switch
        {
            JsonArray array => string.Join(
                "; ",
                array.Select(ToTagString).Where(static x => !string.IsNullOrWhiteSpace(x))
            ),
            JsonValue value => ToScalarString(value),
            _ => token.ToJsonString(),
        };
    }

    private static string ToScalarString(JsonValue value)
    {
        if (value.TryGetValue<string>(out var s))
        {
            return s ?? string.Empty;
        }

        if (value.TryGetValue<bool>(out var b))
        {
            return b.ToString().ToLowerInvariant();
        }

        if (value.TryGetValue<int>(out var i))
        {
            return i.ToString();
        }

        if (value.TryGetValue<long>(out var l))
        {
            return l.ToString();
        }

        if (value.TryGetValue<double>(out var d))
        {
            return d.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        if (value.TryGetValue<decimal>(out var m))
        {
            return m.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        return value.ToJsonString();
    }
}
