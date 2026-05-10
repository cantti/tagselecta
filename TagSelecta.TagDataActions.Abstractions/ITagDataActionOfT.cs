namespace TagSelecta.TagDataActions.Abstractions;

public interface ITagDataAction<TSettings> : ITagDataAction
    where TSettings : TagDataActionSettings
{
    Task ITagDataAction.Execute(ITagDataActionExecuteContext context, CancellationToken token)
    {
        return Execute(
            new TagDataActionExecuteContext<TSettings>
            {
                Settings = (TSettings)context.Settings,
                Target = context.Target,
            },
            token
        );
    }

    Task<bool> ITagDataAction.BeforeExecute(TagDataActionSettings settings, CancellationToken token)
    {
        return BeforeExecute((TSettings)settings, token);
    }

    Task<bool> BeforeExecute(TSettings settings, CancellationToken token);

    Task Execute(TagDataActionExecuteContext<TSettings> context, CancellationToken token);
}
