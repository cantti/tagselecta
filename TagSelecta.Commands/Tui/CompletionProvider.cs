using System.Reflection;
using System.Text.RegularExpressions;
using TagSelecta.Commands.Tui.TuiCommands;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.Commands.Tui;

public class CompletionProvider : ICompletionProvider
{
    private readonly List<string> _keywords = [];

    public CompletionProvider(
        IEnumerable<ITagDataAction> tagDataActions,
        IEnumerable<ITuiCommand> tuiCommands
    )
    {
        var actionKeywords = GetActions(tagDataActions);
        _keywords.AddRange(actionKeywords);
        var commandKeywords = GetTuiCommands(tuiCommands);
        _keywords.AddRange(commandKeywords);
    }

    private IEnumerable<string> GetTuiCommands(IEnumerable<ITuiCommand> tuiCommands)
    {
        return tuiCommands
            .Select(command => command.GetType())
            .Select(type => type.GetCustomAttribute<TuiCommandAttribute>())
            .OfType<TuiCommandAttribute>()
            .Select(attr => attr.Names[0]);
    }

    private IEnumerable<string> GetActions(IEnumerable<ITagDataAction> actions)
    {
        return actions
            .Select(action => action.GetType())
            .Select(type => type.GetCustomAttribute<TagDataActionNameAttribute>())
            .OfType<TagDataActionNameAttribute>()
            .Select(attr => attr.Name);
    }

    public string GetCompletion(string input, int cursorPos)
    {
        if (IsCursorInsideDoubleQuotes(input, cursorPos))
        {
            return "";
        }

        // only consider text to the left of the cursor for completion
        var leftOfCursor = input[..cursorPos];

        // get last word
        var word = Regex.Match(leftOfCursor, "[a-zA-Z]+$").Value;

        if (string.IsNullOrEmpty(word))
        {
            return "";
        }
        var completion = _keywords.FirstOrDefault(c => c.StartsWith(word));
        return completion is not null ? completion[word.Length..] : "";
    }

    private bool IsCursorInsideDoubleQuotes(string input, int cursorPos)
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
