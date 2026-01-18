namespace TagSelecta.Shared.TagDataActions;

public class TagDataActionExecuteContext<TSettings> : ITagDataActionExecuteContext
    where TSettings : TagDataActionSettings
{
    public required ITagDataActionTarget Target { get; set; }
    public required int TargetIndex { get; set; }
    public required IEnumerable<ITagDataActionSnapshot> Files { get; set; }
    public required TSettings Settings { get; set; }
    TagDataActionSettings ITagDataActionExecuteContext.Settings => Settings;
}
