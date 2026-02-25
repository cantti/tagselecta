using System.Reflection;
using System.Text.RegularExpressions;
using Spectre.Console.Cli;
using TagSelecta.Shared.Exceptions;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.Commands.Tui;

public class CompletionProvider : ICompletionProvider
{
    private readonly List<string> _commands = [];
    private List<ActionInfo> _actions = [];

    public CompletionProvider(IEnumerable<ITagDataAction> tagDataActions)
    {
        AddActions(tagDataActions);
    }

    public IEnumerable<string> GetCompletions(string input, int cursorPos)
    {
        if (IsCursorInsideDoubleQuotes(input, cursorPos))
        {
            return [];
        }

        // only consider text to the left of the cursor for completion
        var leftOfCursor = input[..cursorPos];

        // only complete words or after space. ignore things like &, =
        if (input != "" && !Regex.IsMatch(leftOfCursor, @"[a-z\s]+$"))
        {
            return [];
        }

        // get last word
        var word = Regex.Match(leftOfCursor, "[a-zA-Z]+$").Value;

        var currentCommand = GetCurrentCommand(leftOfCursor);

        return currentCommand.IsTyping
            ? GetCommandCompletion(word)
            : GetOptionCompletion(currentCommand.Command, word);
    }

    private void AddActions(IEnumerable<ITagDataAction> actions)
    {
        foreach (var action in actions)
        {
            var nameAttribute = action.GetType().GetCustomAttribute<TagDataActionNameAttribute>();
            if (nameAttribute is null)
            {
                continue;
            }

            // use only full names for command completion
            _commands.Add(nameAttribute.Name);

            var settingsType = GetSettingsTypeFromAction(action.GetType());
            var props = settingsType.GetProperties();
            List<OptionInfo> options = [];
            foreach (var prop in props)
            {
                var attr = prop.GetCustomAttribute<CommandOptionAttribute>();
                if (attr is null)
                {
                    continue;
                }

                // use only long names for option completion and ignore "yes"
                options.AddRange(
                    attr.LongNames.Where(x => x != nameof(TagDataActionSettings.Yes).ToLower())
                        .Select(x => new OptionInfo(x, prop.PropertyType == typeof(bool)))
                );
            }

            options = options.OrderBy(x => x.Name).ToList();

            List<string> names = [nameAttribute.Name];
            if (nameAttribute.Alias is not null)
            {
                names.Add(nameAttribute.Alias);
            }

            _actions.Add(new(names, options));
        }

        _actions = _actions.OrderBy(x => x.Names[0]).ToList();
    }

    private static Type GetSettingsTypeFromAction(Type? actionType)
    {
        while (actionType != null)
        {
            if (
                actionType.IsGenericType
                && actionType.GetGenericTypeDefinition() == typeof(TagDataAction<>)
            )
            {
                return actionType.GetGenericArguments()[0];
            }

            actionType = actionType.BaseType!;
        }

        throw new TagSelectaException(
            $"{actionType} does not inherit from TagDataAction<TSettings>"
        );
    }

    private IEnumerable<string> GetCommandCompletion(string word)
    {
        return _commands.Where(c => c.StartsWith(word)).Select(x => x[word.Length..]);
    }

    private IEnumerable<string> GetOptionCompletion(string currentCommand, string word)
    {
        var action = _actions.FirstOrDefault(a => a.Names.Contains(currentCommand));
        return action is null
            ? []
            : action
                .Options.Where(o => o.Name.StartsWith(word))
                .Select(x => !x.IsFlag ? $"{x.Name[word.Length..]}=" : x.Name[word.Length..]);
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

    private static bool IsCursorInsideDoubleQuotes(string input, int cursorPos)
    {
        // We consider quotes up to (but not including) CursorPos.
        // Escaped quotes \" do not toggle quote state.
        var inQuotes = false;
        var escaped = false;

        var limit = Math.Clamp(cursorPos, 0, input.Length);

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
            }
        }

        return inQuotes;
    }

    private record ActionInfo(List<string> Names, List<OptionInfo> Options);

    private record OptionInfo(string Name, bool IsFlag);
}
