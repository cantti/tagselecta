namespace TagSelecta.Shared.TagDataActions;

public interface ITagDataActionExecuteContext
{
    ITagDataActionTarget Target { get; }

    TagDataActionSettings Settings { get; }
}
