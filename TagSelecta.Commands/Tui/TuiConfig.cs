namespace TagSelecta.Commands.Tui;

public class TuiConfig
{
    public double FileListRatio { get; init; }
    public bool AutoCompletionEnabled { get; init; }
    public bool TreeEnabled { get; init; }
    public string StartupCommand { get; set; } = "";
    public Dictionary<string, string> FieldIcons { get; set; } = new();
    public Appearance Appearance { get; init; } = new();
}
