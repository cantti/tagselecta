using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TagSelecta.TagDataActions.Json;

public static class JsonPathValueResolver
{
    public static string GetValue(JToken token, string path)
    {
        return string.Join("; ", GetValues(token, path));
    }

    public static string GetIndexedValue(JToken token, string path, int index)
    {
        if (index < 0)
        {
            return string.Empty;
        }

        var values = token
            .SelectTokens(path, false)
            .Select(ToTagString)
            .Where(static x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        return index < values.Count ? values[index] : string.Empty;
    }

    public static List<string> GetValues(JToken token, string path)
    {
        return token
            .SelectTokens(path, false)
            .Select(ToTagString)
            .Where(static x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public static string ToTagString(JToken? token)
    {
        if (token is null)
        {
            return string.Empty;
        }

        return token.Type switch
        {
            JTokenType.Null => string.Empty,
            JTokenType.Undefined => string.Empty,
            JTokenType.String => token.Value<string>() ?? string.Empty,
            JTokenType.Array => string.Join(
                "; ",
                token.Children().Select(ToTagString).Where(static x => !string.IsNullOrWhiteSpace(x))
            ),
            _ => token.ToString(Formatting.None),
        };
    }
}
