using System.Reflection;
using System.Text.Json;
using TagSelecta.Cli.Commands.Set;

namespace TagSelecta.Cli.Tui;

public record ActionInDispatcher(string[] Names, ITagDataAction Action);

public sealed class ActionDispatcher
{
    private readonly List<ActionInDispatcher> _actions = [];

    public ActionDispatcher(IEnumerable<ITagDataAction> handlers)
    {
        foreach (var handler in handlers)
        {
            var type = handler.GetType();

            var attr = type.GetCustomAttribute<TagDataActionAttribute>();
            if (attr == null)
                continue;

            _actions.Add(new ActionInDispatcher(attr.Names, handler));
        }
    }

    public async Task Dispatch(
        ActionRequest request,
        IFileContext? current,
        IEnumerable<IFileContext>? files,
        DispatchType type
    )
    {
        var action = _actions.SingleOrDefault(x => x.Names.Contains(request.ActionName));
        if (action is not null)
        {
            var settingsType = GetSettingsTypeFromAction(action.Action.GetType());

            var baseSettings = (BaseSettings)(
                Activator.CreateInstance(settingsType)
                ?? throw new InvalidOperationException("Failed to create settings instance")
            );

            baseSettings.ParseTuiArgs(request.Args);

            if (type == DispatchType.BeforeProcess)
            {
                await action.Action.BeforeProcessTagDataAsync(baseSettings);
            }
            else
            {
                await action.Action.ProcessTagDataAsync(current!, files!, baseSettings);
            }
        }
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
