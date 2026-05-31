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

    public static string GetFullPath(string path)
    {
        return Path.GetFullPath(path);
    }

    public static string ExpandToFullPath(string path)
    {
        return Path.GetFullPath(Expand(path));
    }

    public static string GetFullPath(string path, string basePath)
    {
        return Path.GetFullPath(path, basePath);
    }

    public static string GetExtension(string path)
    {
        return Path.GetExtension(path);
    }

    public static string GetFileNameWithoutExtension(string path)
    {
        return Path.GetFileNameWithoutExtension(path);
    }

    public static string GetFileName(string path)
    {
        return Path.GetFileName(path);
    }

    public static string? GetDirectoryName(string path)
    {
        return Path.GetDirectoryName(path);
    }

    public static string Combine(string path1, string path2)
    {
        return Path.Combine(path1, path2);
    }

    public static string Combine(string path1, string path2, string path3)
    {
        return Path.Combine(path1, path2, path3);
    }

    public static string? GetPathRoot(string path)
    {
        return Path.GetPathRoot(path);
    }

    public static string GetTempPath()
    {
        return Path.GetTempPath();
    }

    public static bool Exists(string path)
    {
        return Path.Exists(path);
    }

    public static char[] GetInvalidFileNameChars()
    {
        return Path.GetInvalidFileNameChars();
    }
}
