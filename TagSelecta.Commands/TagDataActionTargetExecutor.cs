using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.Commands;

public class TagDataActionTargetExecutor
{
    public async Task ExecuteTagDataAction(
        TagDataActionTarget target,
        ITagDataAction action,
        TagDataActionSettings settings,
        CancellationToken token
    )
    {
        target.ResetError();
        try
        {
            await action.Execute(
                new TagDataActionExecuteContext { Settings = settings, Target = target },
                token
            );
        }
        catch (Exception ex)
        {
            target.MarkError(ex);
        }
    }
}
