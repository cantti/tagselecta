namespace TagSelecta.Tagging;

[AttributeUsage(AttributeTargets.Property)]
public class TagDataFieldAttribute(string label) : Attribute
{
    public string Label { get; } = label;
}
