namespace TagSelecta.Tagging;

[AttributeUsage(AttributeTargets.Property)]
public class EditableAttribute(string? label = null) : Attribute
{
    public string? Label { get; } = label;
}
