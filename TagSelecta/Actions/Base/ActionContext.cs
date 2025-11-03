using TagSelecta.BaseCommands;

namespace TagSelecta.Actions.Base;

public class ActionContext<TSettings>
    where TSettings : FileSettings
{
    public required TSettings Settings { get; init; }
    public List<string> Files { get; init; } = [];
    public required Func<bool> ConfirmPrompt { get; init; }
    public required Action Skip { get; init; }
    public required Action Cancel { get; set; }
}
