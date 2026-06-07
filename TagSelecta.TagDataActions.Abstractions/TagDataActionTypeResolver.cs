using TagSelecta.Shared.Exceptions;

namespace TagSelecta.TagDataActions.Abstractions;

public static class TagDataActionTypeResolver
{
    public static Type GetSettingsType(Type actionType)
    {
        if (actionType is null)
        {
            throw new TagSelectaException("Action type cannot be null");
        }

        var interfaceMatch = actionType
            .GetInterfaces()
            .FirstOrDefault(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ITagDataAction<>)
            );

        if (interfaceMatch is not null)
        {
            return interfaceMatch.GetGenericArguments()[0];
        }

        throw new TagSelectaException($"{actionType} does not implement ITagDataAction<TSettings>");
    }
}
