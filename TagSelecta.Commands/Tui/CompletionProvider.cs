using System.Reflection;
using System.Text.RegularExpressions;
using Spectre.Console.Cli;
using TagSelecta.Shared.Exceptions;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.Commands.Tui;

public class CompletionProvider : ICompletionProvider
{
    private readonly List<(string[] Names, string[] Options)> _actions = [];
    private readonly List<string> _commands = [];

    public CompletionProvider(IEnumerable<ITagDataAction> tagDataActions)
    {
        AddActions(tagDataActions);
    }

    public string GetCompletion(string input, int cursorPos, int completionIndex)
    {
        if (IsCursorInsideDoubleQuotes(input, cursorPos))
        {
            return "";
        }

        // only consider text to the left of the cursor for completion
        var leftOfCursor = input[..cursorPos];

        if (leftOfCursor.EndsWith('=') || leftOfCursor.EndsWith('"') || leftOfCursor.EndsWith('&'))
        {
            return "";
        }

        // get last word
        var word = Regex.Match(leftOfCursor, "[a-zA-Z]+$").Value;

        var currentCommand = GetCurrentCommand(leftOfCursor);

        return currentCommand.IsTyping
            ? GetCommandCompletion(word, completionIndex)
            : GetOptionCompletion(currentCommand.Command, word, completionIndex);
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

            List<string> names = [nameAttribute.Name];
            if (nameAttribute.Alias is not null)
            {
                names.Add(nameAttribute.Alias);
            }

            _commands.AddRange(names);
            var settingsType = GetSettingsTypeFromAction(action.GetType());
            var props = settingsType.GetProperties();
            List<string> options = [];
            foreach (var prop in props)
            {
                var attr = prop.GetCustomAttribute<CommandOptionAttribute>();
                if (attr is null)
                {
                    continue;
                }

                options.AddRange(attr.LongNames);
            }

            options = options.OrderBy(x => x).ToList();

            _actions.Add((names.ToArray(), options.ToArray()));
        }
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

    private string GetCommandCompletion(string word, int completionIndex)
    {
        var candidates = _commands.Where(c => c.StartsWith(word)).ToList();
        if (candidates.Count == 0)
        {
            return "";
        }

        var completion = candidates[completionIndex % candidates.Count];
        return completion[word.Length..];
    }

    private string GetOptionCompletion(string currentCommand, string word, int completionIndex)
    {
        var action = _actions.FirstOrDefault(a => a.Names.Contains(currentCommand));
        if (action == default)
        {
            return "";
        }

        var candidates = action.Options.Where(o => o.StartsWith(word)).ToList();
        if (candidates.Count == 0)
        {
            return "";
        }

        var completion = candidates[completionIndex % candidates.Count];
        return completion[word.Length..];
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
}
