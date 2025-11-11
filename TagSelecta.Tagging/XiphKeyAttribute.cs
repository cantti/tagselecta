namespace TagSelecta.Tagging;

[AttributeUsage(AttributeTargets.Property)]
public class XiphKeyAttribute(params string[] keys) : Attribute
{
    public string[] Keys { get; } = keys;
}
