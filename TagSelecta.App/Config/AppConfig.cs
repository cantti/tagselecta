using TagSelecta.Commands.Tui.TuiCommands;

namespace TagSelecta.App.Config;

public class AppConfig
{
    public GeneralConfig General { get; set; } = new();
    public Dictionary<string, string[]> Macros { get; set; } = new();
}
