namespace TagSelecta.TagDataActions.Abstractions;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class TagDataActionNameAttribute(string name, string? alias = null) : Attribute
{
    public string Name { get; } = name;
    public string? Alias { get; } = alias;
}
