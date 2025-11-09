using TagSelecta.BaseCommands;

namespace TagSelecta.TagDataActions;

public interface ITagDataAction<TSettings>
    where TSettings : BaseSettings
{
    bool CompareBeforeWriteTagData => true;

    Task<bool> BeforeProcessTagData(TagDataActionContext<TSettings> context)
    {
        return Task.FromResult(true);
    }

    Task<ActionStatus> ProcessTagData(TagDataActionContext<TSettings> context);

    Task BeforeWriteTagData(TagDataActionContext<TSettings> context)
    {
        return Task.CompletedTask;
    }
}
