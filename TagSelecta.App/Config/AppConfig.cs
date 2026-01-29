namespace TagSelecta.App.Config;

public class AppConfig
{
    public GeneralConfig General { get; set; } = new();
    public Dictionary<string, string> Macro { get; set; } = new();
}
