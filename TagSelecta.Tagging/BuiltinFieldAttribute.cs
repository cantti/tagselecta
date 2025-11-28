namespace TagSelecta.Tagging;

[AttributeUsage(AttributeTargets.Property)]
public class BuiltinFieldAttribute(string label) : Attribute
{
    public string Label { get; } = label;
}
