namespace TagSelecta.TagDataActions.Abstractions;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class TagDataActionInfoAttribute(string name, string? alias = null) : Attribute
{
    public string Name { get; } = name;
    public string? Alias { get; } = alias;
    public bool AllowRemainingArguments { get; set; }
    public FieldNameCompletion FieldNameCompletion { get; set; } = FieldNameCompletion.Disabled;
}

public enum FieldNameCompletion
{
    Disabled,
    Boolean,
    String,
}
