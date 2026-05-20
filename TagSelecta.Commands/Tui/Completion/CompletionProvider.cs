using System.Reflection;
using Spectre.Console.Cli;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.Commands.Tui.Completion;

public class CompletionProvider : ICompletionProvider
{
    private readonly IEnumerable<ITagDataAction> _tagDataActions;

    private readonly List<CompletionSpec> _completionSpecs = [];

    public CompletionProvider(IEnumerable<ITagDataAction> tagDataActions)
    {
        _tagDataActions = tagDataActions;
    }

    public void GenerateCompletions(IEnumerable<string> fieldNames)
    {
        foreach (var action in _tagDataActions)
        {
            var infoAttribute = action.GetType().GetCustomAttribute<TagDataActionInfoAttribute>();
            if (infoAttribute is null)
            {
                continue;
            }

            var settingsType = TagDataActionTypeResolver.GetSettingsType(action.GetType());
            var settingsProps = settingsType.GetProperties();

            List<OptionSpec> options = [];

            foreach (var prop in settingsProps)
            {
                var attr = prop.GetCustomAttribute<CommandOptionAttribute>();
                if (attr is null)
                {
                    continue;
                }

                // use only long names for option completion and ignore "yes"
                options.AddRange(
                    attr.LongNames.Where(x =>
                            !x.Equals(
                                nameof(TagDataActionSettings.Yes),
                                StringComparison.CurrentCultureIgnoreCase
                            )
                        )
                        .Select(x => new OptionSpec(x, prop.PropertyType == typeof(bool)))
                );
            }

            if (infoAttribute.FieldNameCompletion != FieldNameCompletion.Disabled)
            {
                options.AddRange(
                    fieldNames
                        .Where(x => options.All(x2 => x2.Name != x))
                        .Select(x => new OptionSpec(
                            x,
                            infoAttribute.FieldNameCompletion == FieldNameCompletion.Boolean
                        ))
                );
            }

            List<string> commandNames = [infoAttribute.Name];

            if (infoAttribute.Alias is not null)
            {
                commandNames.Add(infoAttribute.Alias);
            }

            _completionSpecs.Add(new CompletionSpec(commandNames, options));
        }

        _completionSpecs.Add(new CompletionSpec(["version"], []));
        _completionSpecs.Add(new CompletionSpec(["open"], []));
    }

    public IEnumerable<string> GetCompletions(string input, int cursorPos)
    {
        var context = GetCursorContext(input, cursorPos);
        if (context.DisableCompletion)
        {
            return [];
        }

        var currentCommand = GetCurrentCommand(context.LeftOfCursor);

        return currentCommand.IsTyping
            ? GetCommandCompletion(context.Token)
            : GetOptionCompletion(currentCommand.Command, context.Token);
    }

    private IEnumerable<string> GetCommandCompletion(string word)
    {
        return _completionSpecs
            // use only long names
            .Select(x => x.Command[0])
            .Where(c => c.StartsWith(word))
            .OrderBy(x => x)
            .Select(x => x[word.Length..]);
    }

    private IEnumerable<string> GetOptionCompletion(string currentCommand, string word)
    {
        var action = _completionSpecs.FirstOrDefault(a => a.Command.Contains(currentCommand));
        var unescapedWord = UnescapeKeyToken(word);
        return action is null
            ? []
            : action
                .Options.Where(o => o.Name.StartsWith(unescapedWord))
                .OrderBy(x => x.Name)
                .Select(x =>
                {
                    var suffix = x.Name[unescapedWord.Length..];
                    var escapedSuffix = EscapeKeyToken(suffix);
                    return !x.Boolean ? $"{escapedSuffix}=" : escapedSuffix;
                });
    }

    private (string Command, bool IsTyping) GetCurrentCommand(string leftOfCursor)
    {
        var idx = leftOfCursor.LastIndexOf("&&", StringComparison.Ordinal);
        var start = idx >= 0 ? idx + 2 : 0; // after &&
        while (start < leftOfCursor.Length && char.IsWhiteSpace(leftOfCursor[start]))
        {
            start++;
        }

        var end = start;
        while (end < leftOfCursor.Length && !char.IsWhiteSpace(leftOfCursor[end]))
        {
            end++;
        }

        var command = start < end ? leftOfCursor[start..end] : "";
        var isTyping = end == leftOfCursor.Length;
        return (command, isTyping);
    }

    private static CursorContext GetCursorContext(string input, int cursorPos)
    {
        var limit = Math.Clamp(cursorPos, 0, input.Length);
        var leftOfCursor = input[..limit];

        var inQuotes = false;
        var escaped = false;
        var tokenStart = 0;

        for (var i = 0; i < limit; i++)
        {
            var ch = input[i];

            if (escaped)
            {
                escaped = false;
                continue;
            }

            if (ch == '\\')
            {
                escaped = true;
                continue;
            }

            if (ch == '"')
            {
                inQuotes = !inQuotes;
                continue;
            }

            if (!inQuotes && char.IsWhiteSpace(ch))
            {
                tokenStart = i + 1;
            }
        }

        var token = leftOfCursor[tokenStart..];
        var disableCompletion = inQuotes || token.Contains('=');
        return new CursorContext(leftOfCursor, token, disableCompletion);
    }

    private static string EscapeKeyToken(string value)
    {
        return value.Replace("\\", "\\\\").Replace(" ", "\\ ");
    }

    private static string UnescapeKeyToken(string value)
    {
        var result = new char[value.Length];
        var idx = 0;
        var escaped = false;

        foreach (var ch in value)
        {
            if (escaped)
            {
                result[idx++] = ch;
                escaped = false;
                continue;
            }

            if (ch == '\\')
            {
                escaped = true;
                continue;
            }

            result[idx++] = ch;
        }

        if (escaped)
        {
            result[idx++] = '\\';
        }

        return new string(result, 0, idx);
    }
}
