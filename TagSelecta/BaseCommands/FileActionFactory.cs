using Microsoft.Extensions.DependencyInjection;
using TagSelecta.Actions.Base;

namespace TagSelecta.BaseCommands;

public class FileActionFactory<TAction, TSettings>(IServiceProvider provider)
    where TAction : IFileAction<TSettings>
    where TSettings : FileSettings
{
    private readonly IServiceProvider _provider = provider;

    public TAction Create(ActionContext<TSettings> settings)
    {
        try
        {
            return ActivatorUtilities.CreateInstance<TAction>(_provider, settings);
        }
        catch (InvalidOperationException)
        {
            // Fall back to constructor without settings
            return ActivatorUtilities.CreateInstance<TAction>(_provider);
        }
    }
}
