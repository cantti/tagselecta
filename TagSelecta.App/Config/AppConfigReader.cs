using Tomlyn;

namespace TagSelecta.App.Config;

public static class AppConfigReader
{
    public static AppConfig Read()
    {
        var configPath = GetConfigPath();
        var tomlText = File.ReadAllText(configPath);
        var config = Toml.ToModel<AppConfig>(tomlText);
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
            baseConfigDir = Path.Combine(appData, appName);
        }
        else
        {
            var xdg = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
            if (!string.IsNullOrWhiteSpace(xdg))
            {
                baseConfigDir = Path.Combine(xdg, appName);
            }
            else
            {
                var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                baseConfigDir = Path.Combine(home, ".config", appName);
            }
        }

        Directory.CreateDirectory(baseConfigDir);

        var path = Path.Combine(baseConfigDir, fileName);
        if (!File.Exists(path))
        {
            File.WriteAllText(path, "");
        }

        return path;
    }
}
