using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using TagSelecta.Commands.Cli.ExecuteTagDataAction;
using TagSelecta.Shared.Exceptions;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.Commands;

public static class CommandConfiguratorExtensions
{
    public static ICommandConfigurator AddTagDataAction<TAction>(
        this IConfigurator configurator,
        IServiceCollection services
    )
        where TAction : ITagDataAction
    {
        // find TagDataActionAttribute
        var attr =
            typeof(TAction).GetCustomAttribute<TagDataActionInfoAttribute>()
            ?? throw new TagSelectaException("TagDataActionAttribute not found");

        // extract TSettings from TAction
        var settingsType = TagDataActionTypeResolver.GetSettingsType(typeof(TAction));

        // make TagDataAction<TSettings>
        var actionType = typeof(ITagDataAction<>).MakeGenericType(settingsType);

        // make TagDataCommand<TSettings>
        var commandType = typeof(ExecuteTagDataActionCommand<>).MakeGenericType(settingsType);

        services.AddTransient(actionType, typeof(TAction));

        services.AddTransient(typeof(ITagDataAction), typeof(TAction));

        // find AddCommand<T>(string)
        var addCommandMethod = typeof(IConfigurator)
            .GetMethods()
            .Single(m =>
                m.Name == nameof(configurator.AddCommand)
                && m.IsGenericMethodDefinition
                && m.GetGenericArguments().Length == 1
                && m.GetParameters().Length == 1
                && m.GetParameters()[0].ParameterType == typeof(string)
            );

        // call
        var commandConfigurator = (ICommandConfigurator)
            addCommandMethod.MakeGenericMethod(commandType).Invoke(configurator, [attr.Name])!;

        // add alias
        if (!string.IsNullOrWhiteSpace(attr.Alias))
        {
            commandConfigurator.WithAlias(attr.Alias);
        }

        return commandConfigurator;
    }

}
