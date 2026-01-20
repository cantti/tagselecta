namespace TagSelecta.TagDataActions.Abstractions;

public interface ITagDataActionExecuteContext
{
    ITagDataActionTarget Target { get; }

    TagDataActionSettings Settings { get; }
}
