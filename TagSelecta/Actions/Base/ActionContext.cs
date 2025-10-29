using Spectre.Console;

namespace TagSelecta.Actions.Base;

public class ActionContext<TSettings>
{
    public required TSettings Settings { get; init; }
    public List<string> Files { get; init; } = [];
    public required string File { get; init; }
    public required Func<bool> ConfirmPrompt { get; init; }
    public required IAnsiConsole Console { get; init; }
    public required Action Skip { get; init; }
    public required Action ContinuePrompt { get; init; }
}
