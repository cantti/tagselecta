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
            BaseSettings baseSettings;

            if (settingsType == typeof(SetSettings))
            {
                var editSettings = new SetSettings
                {
                    Set = request
                        .Args.Where(x => !x.Key.StartsWith("arg"))
                        .Select(x => $"{x.Key}={x.Value}")
                        .ToArray(),
                    ClearCustom = request.Args.Any(x => x.Value == "clearcustom"),
                };
                baseSettings = editSettings;
            }
            else
            {
                var json = JsonSerializer.Serialize(request.Args);

                baseSettings = (BaseSettings)
                    JsonSerializer.Deserialize(
                        json,
                        settingsType,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    )!;
            }

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
