using System.Reflection;
using Spectre.Console.Cli;
using TagSelecta.Shared.Exceptions;
using TagSelecta.Shared.Tagging;
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

        var actionAttr = action.GetType().GetCustomAttribute<TagDataActionInfoAttribute>()!;

        var settings = CreateSettings(
            action,
            parsedCommand.Options,
            actionAttr.AllowRemainingArguments
        );

        await action.BeforeExecute(settings, token);

        var selectedFilesList = context.SelectedFiles.ToList();

        var executor = new TagDataActionTargetExecutor();

        for (var i = 0; i < selectedFilesList.Count; i++)
        {
            token.ThrowIfCancellationRequested();
            var file = selectedFilesList[i];
            await executor.ExecuteTagDataAction(file, action, settings, token);
            context.Print(
                $"Processed {i + 1} of {selectedFilesList.Count} files. "
                    + $"{selectedFilesList.Count(x => x.Exception is not null)} errors. Type :w to write changes."
            );
        }
    }

    private static TagDataActionSettings CreateSettings(
        ITagDataAction action,
        IEnumerable<ParsedCommandOption> options,
        bool allowRemainingArguments
    )
    {
        var settingsType = TagDataActionTypeResolver.GetSettingsType(action.GetType());
        var baseSettings = (TagDataActionSettings)(
            Activator.CreateInstance(settingsType)
            ?? throw new TagSelectaException("Failed to create settings instance")
        );

        var argList = options.ToList();

        var settingsProps = settingsType.GetProperties();

        foreach (var settingProp in settingsProps)
        {
            var commandOptionAttr = settingProp.GetCustomAttribute<CommandOptionAttribute>();

            if (commandOptionAttr is null)
            {
                continue;
            }

            var matchedArgs = argList
                .Where(x =>
                    commandOptionAttr.LongNames.Contains(x.Key)
                    || commandOptionAttr.ShortNames.Contains(x.Key)
                )
                .ToArray();

            if (matchedArgs.Length == 0)
            {
                if (commandOptionAttr.IsRequired)
                {
                    throw new TagSelectaException(
                        $"Command is missing required argument '{commandOptionAttr.LongNames[0]}'"
                    );
                }

                continue;
            }

            if (settingProp.PropertyType == typeof(string))
            {
                settingProp.SetValue(baseSettings, matchedArgs.First().Value);
            }
            else if (
                settingProp.PropertyType == typeof(string[])
                || settingProp.PropertyType == typeof(List<string>)
                || settingProp.PropertyType == typeof(IEnumerable<string>)
            )
            {
                var values = matchedArgs.Select(x => x.Value).ToList();

                settingProp.SetValue(
                    baseSettings,
                    settingProp.PropertyType == typeof(string[]) ? values.ToArray() : values
                );
            }
            else if (settingProp.PropertyType == typeof(bool))
            {
                var value = matchedArgs[0].Value;

                if (value.TryParseBool(out var parsedValue))
                {
                    settingProp.SetValue(baseSettings, parsedValue);
                }
                else
                {
                    throw new TagSelectaException(
                        $"Invalid value '{value}' for '{matchedArgs[0].Key}'. Expected true/false or 1/0."
                    );
                }
            }
            else
            {
                throw new TagSelectaException(
                    $"Unsupported property type: {settingProp.PropertyType}"
                );
            }

            argList.RemoveAll(x => matchedArgs.Any(matchedArg => matchedArg.Key == x.Key));
        }

        if (argList.Count > 0)
        {
            if (allowRemainingArguments)
            {
                if (baseSettings is not ISettingsWithKey settingsWithKey)
                {
                    throw new TagSelectaException(
                        $"Action settings type '{baseSettings.GetType().Name}' must implement '{nameof(ISettingsWithKey)}' when remaining arguments are allowed."
                    );
                }

                settingsWithKey.Key = settingsWithKey
                    .Key.Concat(argList.Select(x => x.Key))
                    .ToList();

                if (baseSettings is ISettingsWithKeyValue settingsWithKeyValue)
                {
                    settingsWithKeyValue.Value = settingsWithKeyValue
                        .Value.Concat(argList.Select(x => x.Value))
                        .ToList();
                }
            }
            else
            {
                throw new TagSelectaException(
                    $"Unknown arguments: {string.Join(", ", argList.Select(x => x.Key))}"
                );
            }
        }

        var validationResult = baseSettings.Validate();

        if (!validationResult.Successful)
        {
            throw new TagSelectaException(validationResult.Message);
        }

        return baseSettings;
    }
}
