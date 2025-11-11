namespace TagSelecta.Tagging;

[AttributeUsage(AttributeTargets.Property)]
public class PrintableAttribute(string? label = null) : Attribute
{
    public string? Label { get; } = label;
}
