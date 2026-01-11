using TagSelecta.Shared.TagDataActions;

namespace TagSelecta.Tui.TuiCommands;

[TuiCommand("execute")]
public class ExecuteTagDataActionCommand(ITagDataActionFactory actionFactory) : ITuiCommand
{
    public async Task ExecuteAsync(
        ITuiCommandContext context,
        Request request,
        CancellationToken token
    )
    {
        var action = actionFactory.Create(request.Name);

        if (action is null)
        {
            context.Print($"No action found with name {request.Name}");
            return;
        }

        context.Print($"Starting {request.Name} action..");

        await ActionBeforeProcess(action, request, token);

        var selectedOperationsList = context.SelectedOperations.ToList();

        for (int i = 0; i < selectedOperationsList.Count; i++)
        {
            var operation = selectedOperationsList[i];
            try
            {
                await ActionProcess(action, request, operation, context.Operations, token);
                operation.CheckForChanges();
                context.Print($"Updated {i + 1} of {selectedOperationsList.Count} files.");
            }
            catch (Exception ex)
            {
                operation.MarkError(ex);
            }
        }
    }

    private async Task ActionBeforeProcess(
        ITagDataAction action,
        Request request,
        CancellationToken token
    )
    {
        var baseSettings = CreateSettings(action, request.Args);
        await action.BeforeProcessTagDataAsync(baseSettings, token);
    }

    private async Task ActionProcess(
        ITagDataAction action,
        Request request,
        IFileContext current,
        IEnumerable<IFileContext> files,
        CancellationToken token
    )
    {
        var baseSettings = CreateSettings(action, request.Args);
        await action.ProcessTagDataAsync(current, files, baseSettings, token);
    }

    private static TagDataActionSettings CreateSettings(
        ITagDataAction action,
        TagDataActionArg[] args
    )
    {
        var settingsType = GetSettingsTypeFromAction(action.GetType());
        var baseSettings = (TagDataActionSettings)(
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
