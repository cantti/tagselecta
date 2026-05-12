namespace TagSelecta.Commands.Tui.Completion;

public record CursorContext(string LeftOfCursor, string Token, bool DisableCompletion);