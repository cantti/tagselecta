namespace TagSelecta.Commands.Tui;

public interface ICompletionProvider
{
    /// <summary>
    ///     Get completion for text before cursor.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cursorPos"></param>
    /// <param name="completionIndex"></param>
    /// <returns></returns>
    IEnumerable<string> GetCompletions(string input, int cursorPos);
}
