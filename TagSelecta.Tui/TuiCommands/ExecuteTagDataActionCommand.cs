using System.Reflection;
using Spectre.Console.Cli;
using TagSelecta.Shared.Exceptions;
using TagSelecta.Shared.TagDataActions;

namespace TagSelecta.Tui.TuiCommands;

[TuiCommand("execute")]
public class ExecuteTagDataActionCommand(
    ITagDataActionFactory actionFactory,
    ITagDataActionExecutor executor
) : ITuiCommand
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

        var settings = CreateSettings(action, request.Args);

        await action.BeforeExecuteAsync(settings, token);

        var selectedOperationsList = context.SelectedOperations.ToList();

        for (var i = 0; i < selectedOperationsList.Count; i++)
        {
            token.ThrowIfCancellationRequested();
            var operation = selectedOperationsList[i];
            await executor.Execute(
                operation,
                context.Operations.ToList().IndexOf(operation),
                action,
                settings,
                selectedOperationsList,
                token
            );
            context.Print(
                $"Processed {i + 1} of {selectedOperationsList.Count} files. Type :w to write changes."
            );
        }
    }

    private static TagDataActionSettings CreateSettings(
        ITagDataAction action,
        TagDataActionArg[] args
    )
    {
        var settingsType = GetSettingsTypeFromAction(action.GetType());
        var baseSettings = (TagDataActionSettings)(
            Activator.CreateInstance(settingsType)
            ?? throw new TagSelectaException("Failed to create settings instance")
        );

        var argList = args.ToList();

        var props = settingsType.GetProperties();

        foreach (var prop in props)
        {
            var attr = prop.GetCustomAttribute<CommandOptionAttribute>();
            if (attr is null)
            {
                continue;
            }
            var matchedValues = argList
                .Where(x => attr.LongNames.Contains(x.Key) || attr.ShortNames.Contains(x.Key))
                .Select(x => x.Value)
                .ToArray();
            if (matchedValues.Length == 0)
            {
                if (attr.IsRequired)
                {
                    throw new TagSelectaException(
                        $"Command is missing required argument '{attr.LongNames[0]}'"
                    );
                }
                continue;
            }
            if (prop.PropertyType == typeof(string))
            {
                prop.SetValue(baseSettings, matchedValues.First());
            }
            else if (prop.PropertyType == typeof(string[]))
            {
                prop.SetValue(baseSettings, matchedValues);
            }
            else if (prop.PropertyType == typeof(bool))
            {
                prop.SetValue(baseSettings, true);
            }
            else
            {
                throw new TagSelectaException($"Unsupported property type: {prop.PropertyType}");
            }
            argList.RemoveAll(x => matchedValues.Contains(x.Value));
        }

        if (argList.Count > 0)
        {
            throw new TagSelectaException(
                $"Unknown arguments: {string.Join(", ", argList.Select(x => x.Key))}"
            );
        }

        var validationResult = baseSettings.Validate();

        if (!validationResult.Successful)
        {
            throw new TagSelectaException(validationResult.Message);
        }

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

        throw new TagSelectaException(
            $"{actionType} does not inherit from TagDataAction<TSettings>"
        );
    }
}
