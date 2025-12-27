using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using TagSelecta.Cli.Commands.Common;

namespace TagSelecta.Cli;

public static class CommandConfiguratorExtensions
{
    public static ICommandConfigurator AddTagDataAction<TAction>(
        this IConfigurator configurator,
        IServiceCollection services,
        string name
    )
        where TAction : ITagDataAction
    {
        // extract TSettings from TAction
        var settingsType = GetSettingsTypeFromAction(typeof(TAction));

        // make TagDataAction<TSettings>
        var actionType = typeof(TagDataAction<>).MakeGenericType(settingsType);

        // make TagDataCommand<TSettings>
        var commandType = typeof(TagDataCommand<>).MakeGenericType(settingsType);

        services.AddTransient(actionType, typeof(TAction));

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
        return (ICommandConfigurator)
            addCommandMethod.MakeGenericMethod(commandType).Invoke(configurator, [name])!;
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
