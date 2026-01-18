namespace TagSelecta.Shared.TagDataActions;

public interface ITagDataActionExecuteContext
{
    ITagDataActionTarget Target { get; }

    int TargetIndex { get; }

    IEnumerable<ITagDataActionSnapshot> Files { get; }

    TagDataActionSettings Settings { get; }
}