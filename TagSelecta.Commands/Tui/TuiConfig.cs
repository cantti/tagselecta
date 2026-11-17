namespace TagSelecta.Commands.Tui;

public class TuiConfig
{
    public double FileListRatio { get; init; }
    public bool AutoCompletionEnabled { get; init; }
    public bool TreeEnabled { get; init; }
    public string StartupCommand { get; set; } = "";
    public Dictionary<string, string> FieldIcons { get; set; } = new();
    public ColorScheme Colors { get; init; } = new();
    public string SelectionSymbol { get; set; } = "✓";
    public string ChangeSymbol { get; set; } = "→";
    public bool FieldIconsEnabled { get; set; } = false;
}
