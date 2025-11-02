namespace TagSelecta.Actions.Base;

public class ActionBeforeContext<TSettings>
{
    public required TSettings Settings { get; init; }
    public List<string> Files { get; init; } = [];
    public required Action Cancel { get; set; }
}
