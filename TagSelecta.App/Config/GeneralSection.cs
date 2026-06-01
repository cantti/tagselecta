namespace TagSelecta.App.Config;

public class GeneralSection
{
    public bool KeepId3v1 { get; set; } = false;
    public bool Debug { get; set; }
    public double FileListRatio { get; set; } = 0.3;
    public bool AutoCompletionEnabled { get; set; } = true;
    public bool TreeEnabled { get; set; } = true;
    public bool HeaderVisible { get; set; } = true;
    public bool SelectAllOnStartup { get; set; } = false;
}
