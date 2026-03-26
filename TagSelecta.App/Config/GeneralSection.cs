namespace TagSelecta.App.Config;

public class GeneralSection
{
    public bool Debug { get; set; }
    public double FileListRatio { get; set; } = 0.3;
    public bool AutoCompletionEnabled { get; set; } = true;
    public bool TreeEnabled { get; set; } = false;
}
