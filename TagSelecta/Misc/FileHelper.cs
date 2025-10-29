namespace TagSelecta.Misc;

public static class FileHelper
{
    private static readonly HashSet<string> allowedExtensions = [".mp3", ".flac", ".ogg"];

    public static List<string> GetAllAudioFiles(IEnumerable<string> paths, bool recursive = false)
    {
        var files = new List<string>();
        foreach (var path in paths)
        {
            var fullPath = Path.GetFullPath(path);
            if (File.Exists(fullPath))
            {
                if (allowedExtensions.Contains(Path.GetExtension(fullPath).ToLower()))
                {
                    files.Add(fullPath);
                }
            }
            else
            {
                // Common audio file extensions
                string[] audioExtensions = [".mp3", ".flac", ".ogg"];

                var searchOption = recursive
                    ? SearchOption.AllDirectories
                    : SearchOption.TopDirectoryOnly;

                files.AddRange(
                    [
                        .. Directory
                            .GetFiles(fullPath, "*", searchOption)
                            .Where(f =>
                            {
                                var fileName = Path.GetFileName(f);
                                var dirName = new DirectoryInfo(
                                    Path.GetDirectoryName(f) ?? string.Empty
                                ).Name;
                                return !fileName.StartsWith('.')
                                    && !dirName.StartsWith('.')
                                    && audioExtensions.Contains(Path.GetExtension(f).ToLower());
                            })
                            .Order(),
                    ]
                );
            }
        }
        return files;
    }
}
