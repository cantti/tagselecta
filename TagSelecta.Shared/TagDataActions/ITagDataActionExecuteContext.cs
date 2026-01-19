namespace TagSelecta.Shared.TagDataActions;

public interface ITagDataActionExecuteContext
{
    ITagDataActionTarget Target { get; }

    IEnumerable<ITagDataActionFileInfo> DirectoryFiles { get; }

    TagDataActionSettings Settings { get; }
}
