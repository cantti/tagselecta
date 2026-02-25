namespace TagSelecta.Shared.IO;

public static class PathUtils
{
    public static string Expand(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return path;
        }

        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        if (path == "~")
        {
            return home;
        }

        if (!path.StartsWith("~/") && !path.StartsWith("~\\"))
        {
            return path;
        }

        return Path.Combine(home, path[2..]);
    }
}
