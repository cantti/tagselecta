namespace TagSelecta.Commands.Tui;

public class TuiAppConfig
{
    public double FileListRatio { get; init; }
    public bool AutoCompletionEnabled { get; init; }
    public bool TreeEnabled { get; init; }
    public bool HeaderVisible { get; init; }
    public string StartupCommand { get; set; } = "";
}
