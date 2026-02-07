namespace TagSelecta.App.Config;

public class ConfigModel
{
    public GeneralSection General { get; set; } = new();
    public Dictionary<string, string> Macros { get; set; } = new();
}
