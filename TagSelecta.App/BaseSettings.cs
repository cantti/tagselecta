using Spectre.Console.Cli;
using TagSelecta.App.Tui.TuiCommands;

namespace TagSelecta.App;

public abstract class BaseSettings : CommandSettings
{
    [CommandArgument(0, "<path>")]
    public string[] Path { get; set; } = [];

    [CommandOption("--yes")]
    public bool Yes { get; set; }

    public virtual void ParseTuiArgs(IEnumerable<Arg> args) { }
}
