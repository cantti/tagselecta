namespace TagSelecta.Actions.Base;

public class BeforeExecuteContext<TSettings>
{
    public required TSettings Settings { get; init; }
    public List<string> Files { get; init; } = [];
}
