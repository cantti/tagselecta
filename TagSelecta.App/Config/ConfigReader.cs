using TagSelecta.Shared.IO;
using Tomlyn;

namespace TagSelecta.App.Config;

public static class ConfigReader
{
    private const string DefaultConfig = "";

    public static ConfigModel Read()
    {
        var configPath = GetConfigPath();
        var tomlText = File.ReadAllText(configPath);
        var options = new TomlModelOptions { IgnoreMissingProperties = true };
        var config = Toml.ToModel<ConfigModel>(tomlText, configPath, options);
        return config;
    }

    private static string GetConfigPath()
    {
        const string appName = "tagselecta";
        const string fileName = "config.toml";

        string baseConfigDir;

        if (OperatingSystem.IsWindows())
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            baseConfigDir = PathUtils.Combine(appData, appName);
        }
        else
        {
            var xdg = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
            if (!string.IsNullOrWhiteSpace(xdg))
            {
                baseConfigDir = PathUtils.Combine(xdg, appName);
            }
            else
            {
                var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                baseConfigDir = PathUtils.Combine(home, ".config", appName);
            }
        }

        Directory.CreateDirectory(baseConfigDir);

        var path = PathUtils.Combine(baseConfigDir, fileName);
        if (!File.Exists(path))
        {
            File.WriteAllText(path, DefaultConfig);
        }

        return path;
    }
}
