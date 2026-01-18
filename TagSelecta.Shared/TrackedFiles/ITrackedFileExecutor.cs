using TagSelecta.Shared.TagDataActions;

namespace TagSelecta.Shared.TrackedFiles;

public interface ITrackedFileExecutor
{
    void Write(TrackedFile trackedFile);
    Task Execute(
        TrackedFile trackedFile,
        ITagDataAction action,
        ITagDataActionExecuteContext context,
        CancellationToken token
    );
}
