using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.Commands.Tui;

public sealed class TagDataActionFactory : ITagDataActionFactory
{
    private readonly List<(string[] Names, Func<ITagDataAction> Factory)> _factories = [];

    public TagDataActionFactory(IServiceProvider provider)
    {
        var actions = provider.GetServices<ITagDataAction>();
        foreach (var action in actions)
        {
            var type = action.GetType();
            var attr = type.GetCustomAttribute<TagDataActionInfoAttribute>();
            if (attr is null)
            {
                continue;
            }

            var names = new List<string> { attr.Name };
            if (attr.Alias is not null)
            {
                names.Add(attr.Alias);
            }

            _factories.Add(
                (
                    names.ToArray(),
                    () => provider.GetServices<ITagDataAction>().Single(x => x.GetType() == type)
                )
            );
        }
    }

    public ITagDataAction? Create(string name)
    {
        var factory = _factories.SingleOrDefault(f => f.Names.Contains(name));
        return factory.Factory?.Invoke();
    }
}
