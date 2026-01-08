namespace TagSelecta.Cli.Tui.TuiCommands;

public class TagDataCommand(ITagDataActionFactory actionFactory) : ITuiCommand
{
    public async Task ExecuteAsync(ITuiCommandContext context, Request request)
    {
        await ActionBeforeProcess(request);
        await Parallel.ForEachAsync(
            context.Operations.Where(x => x.IsSelected),
            async (operation, _) =>
            {
                try
                {
                    await ActionProcess(request, operation, context.Operations);
                    operation.CheckForChanges();
                }
                catch (Exception ex)
                {
                    operation.MarkError(ex);
                }
            }
        );
    }

    private async Task ActionBeforeProcess(Request request)
    {
        var action = actionFactory.Create(request.Name);
        var baseSettings = CreateSettings(action, request.Args);
        await action.BeforeProcessTagDataAsync(baseSettings);
    }

    private async Task ActionProcess(
        Request request,
        IFileContext current,
        IEnumerable<IFileContext> files
    )
    {
        var action = actionFactory.Create(request.Name);
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
