namespace TagSelecta.Commands.Tui.TuiCommands;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class TuiCommandAttribute : Attribute
{
    public string[] Names { get; }

    public TuiCommandAttribute(params string[] names)
    {
        Names = names;
    }
}
