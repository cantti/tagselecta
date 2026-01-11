namespace TagSelecta.App.Shared;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class TuiTagDataAction : Attribute
{
    public string[] Names { get; }

    public TuiTagDataAction(params string[] names)
    {
        Names = names;
    }
}
