using System.Reflection;
using Spectre.Console.Cli;
using TagSelecta.Shared.Exceptions;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("execute")]
public class ExecuteTagDataActionCommand(ITagDataActionFactory actionFactory) : ITuiCommand
{
    public async Task ExecuteAsync(
        ITuiCommandContext context,
        ParsedCommand parsedCommand,
        CancellationToken token
    )
    {
        var action = actionFactory.Create(parsedCommand.Name);

        if (action is null)
        {
            context.Print($"No action found with name {parsedCommand.Name}");
            return;
        }

        context.Print($"Starting {parsedCommand.Name} action..");

        var settings = CreateSettings(action, parsedCommand.Options);

        await action.BeforeExecute(settings, token);

        var selectedFilesList = context.SelectedFiles.ToList();

        for (var i = 0; i < selectedFilesList.Count; i++)
        {
            token.ThrowIfCancellationRequested();
            var file = selectedFilesList[i];
            await file.ExecuteTagDataAction(
                action,
                new TagDataActionExecuteContext { Settings = settings, Target = file },
                token
            );
            context.Print(
                $"Processed {i + 1} of {selectedFilesList.Count} files. "
                    + $"{selectedFilesList.Count(x => x.Exception is not null)} errors. Type :w to write changes."
            );
        }
    }

    private static TagDataActionSettings CreateSettings(
        ITagDataAction action,
        ParsedCommandOption[] options
    )
    {
        var settingsType = GetSettingsTypeFromAction(action.GetType());
        var baseSettings = (TagDataActionSettings)(
            Activator.CreateInstance(settingsType)
            ?? throw new TagSelectaException("Failed to create settings instance")
        );

        var argList = options.ToList();

        var props = settingsType.GetProperties();

        foreach (var prop in props)
        {
            var attr = prop.GetCustomAttribute<CommandOptionAttribute>();
            if (attr is null)
            {
                continue;
            }

            var matchedArgs = argList
                .Where(x => attr.LongNames.Contains(x.Key) || attr.ShortNames.Contains(x.Key))
                .ToArray();
            if (matchedArgs.Length == 0)
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
                prop.SetValue(baseSettings, matchedArgs.First().Value);
            }
            else if (prop.PropertyType == typeof(string[]))
            {
                prop.SetValue(baseSettings, matchedArgs.Select(x => x.Value).ToArray());
            }
            else if (prop.PropertyType == typeof(bool))
            {
                prop.SetValue(baseSettings, matchedArgs[0].Value is "true" or "1" or "");
            }
            else
            {
                throw new TagSelectaException($"Unsupported property type: {prop.PropertyType}");
            }

            argList.RemoveAll(x => matchedArgs.Any(matchedArg => matchedArg.Key == x.Key));
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
