namespace TagSelecta.TagDataActions.Abstractions;

public class TagDataActionExecuteContext<TSettings> : ITagDataActionExecuteContext
    where TSettings : TagDataActionSettings
{
    public required ITagDataActionTarget Target { get; init; }
    public required TSettings Settings { get; init; }
    TagDataActionSettings ITagDataActionExecuteContext.Settings => Settings;
}
