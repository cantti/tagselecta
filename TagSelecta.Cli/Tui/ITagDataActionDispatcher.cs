namespace TagSelecta.Cli.Tui;

public interface ITagDataActionDispatcher
{
    Task BeforeProcess(ActionRequest request);

    Task Process(ActionRequest request, IFileContext current, IEnumerable<IFileContext> files);
}
