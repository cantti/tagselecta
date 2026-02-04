namespace TagSelecta.TagDataActions.Abstractions;

public class TagDataActionExecuteContext<TSettings> : ITagDataActionExecuteContext
    where TSettings : TagDataActionSettings
{
    public required TSettings Settings { get; init; }
    public required ITagDataActionTarget Target { get; init; }
    TagDataActionSettings ITagDataActionExecuteContext.Settings => Settings;
}
