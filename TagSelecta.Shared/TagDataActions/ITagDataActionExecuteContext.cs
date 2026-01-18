namespace TagSelecta.Shared.TagDataActions;

public interface ITagDataActionExecuteContext
{
    ITagDataActionTarget Target { get; }

    IEnumerable<ITagDataActionFileInfo> Files { get; }

    TagDataActionSettings Settings { get; }
}
