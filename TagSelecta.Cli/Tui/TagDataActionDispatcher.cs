namespace TagSelecta.Cli.Tui;

public class TagDataActionDispatcher(ITagDataActionFactory actionFactory) : ITagDataActionDispatcher
{
    public async Task BeforeProcess(ActionRequest request)
    {
        var action = actionFactory.Create(request.ActionName);
        var baseSettings = CreateSettings(action, request.Args);
        await action.BeforeProcessTagDataAsync(baseSettings);
    }

    public async Task Process(
        ActionRequest request,
        IFileContext current,
        IEnumerable<IFileContext> files
    )
    {
        var action = actionFactory.Create(request.ActionName);
        var baseSettings = CreateSettings(action, request.Args);
        await action.ProcessTagDataAsync(current, files, baseSettings);
    }

    private static BaseSettings CreateSettings(
        ITagDataAction action,
        Dictionary<string, string> args
    )
    {
        var settingsType = GetSettingsTypeFromAction(action.GetType());
        var baseSettings = (BaseSettings)(
            Activator.CreateInstance(settingsType)
            ?? throw new InvalidOperationException("Failed to create settings instance")
        );
        baseSettings.ParseTuiArgs(args);
        return baseSettings;
    }

    private static Type GetSettingsTypeFromAction(Type? actionType)
    {
        while (actionType != null)
        {
            if (
                actionType.IsGenericType
                && actionType.GetGenericTypeDefinition() == typeof(TagDataAction<>)
            )
            {
                return actionType.GetGenericArguments()[0];
            }

            actionType = actionType.BaseType!;
        }

        throw new InvalidOperationException(
            $"{actionType} does not inherit from TagDataAction<TSettings>"
        );
    }
}
