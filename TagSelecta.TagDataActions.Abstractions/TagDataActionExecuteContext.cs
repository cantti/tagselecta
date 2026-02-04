namespace TagSelecta.TagDataActions.Abstractions;

public class TagDataActionExecuteContext : ITagDataActionExecuteContext
{
    public required TagDataActionSettings Settings { get; init; }
    public required ITagDataActionTarget Target { get; init; }
    TagDataActionSettings ITagDataActionExecuteContext.Settings => Settings;
}
