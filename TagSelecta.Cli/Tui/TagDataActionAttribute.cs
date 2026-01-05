namespace TagSelecta.Cli.Tui;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class TagDataActionAttribute : Attribute
{
    public string[] Names { get; }

    public TagDataActionAttribute(params string[] names)
    {
        Names = names;
    }
}
