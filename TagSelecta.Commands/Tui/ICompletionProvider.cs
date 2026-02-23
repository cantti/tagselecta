namespace TagSelecta.Commands.Tui;

public interface ICompletionProvider
{
    string GetCompletion(string input, int cursorPos, int completionIndex);
}
