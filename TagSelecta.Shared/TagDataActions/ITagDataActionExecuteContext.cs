namespace TagSelecta.Shared.TagDataActions;

public interface ITagDataActionExecuteContext
{
    ITagDataActionTarget Target { get; }

    IEnumerable<ITagDataActionSnapshot> Files { get; }

    TagDataActionSettings Settings { get; }
}
