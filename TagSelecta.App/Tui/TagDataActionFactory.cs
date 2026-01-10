using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace TagSelecta.App.Tui;

public sealed class TagDataActionFactory : ITagDataActionFactory
{
    private readonly List<(string[] Names, Func<ITagDataAction> Factory)> _factories = [];

    public TagDataActionFactory(IEnumerable<ITagDataAction> actions, IServiceProvider provider)
    {
        foreach (var action in actions)
        {
            var type = action.GetType();
            var attr = type.GetCustomAttribute<TagDataActionAttribute>();
            if (attr is null || attr.Names.Length == 0)
                continue;

            _factories.Add((attr.Names, () => (ITagDataAction)provider.GetRequiredService(type)));
        }
    }

    public ITagDataAction? Create(string name)
    {
        var factory = _factories.SingleOrDefault(f => f.Names.Contains(name));
        return factory.Factory?.Invoke();
    }
}
