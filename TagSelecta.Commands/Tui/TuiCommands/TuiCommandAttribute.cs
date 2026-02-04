namespace TagSelecta.Commands.Tui.TuiCommands;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class TuiCommandAttribute : Attribute
{
    public TuiCommandAttribute(params string[] names)
    {
        Names = names;
    }

    public string[] Names { get; }
}
