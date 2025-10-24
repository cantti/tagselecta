namespace AudioTagCli.Misc;

public static class Helper
{
    private static readonly HashSet<string> allowedExtensions = [".mp3", ".flac", ".ogg"];

    public static List<string> GetAllAudioFiles(string path, bool recursive = false)
    {
        path = Path.GetFullPath(path);
        if (File.Exists(path))
        {
            if (allowedExtensions.Contains(Path.GetExtension(path).ToLower()))
            {
                return [path];
            }
            else
            {
                return [];
            }
        }
        else
        {
            // Common audio file extensions
            string[] audioExtensions = [".mp3", ".flac", ".ogg"];

            // Decide search option based on the flag
            var searchOption = recursive
                ? SearchOption.AllDirectories
                : SearchOption.TopDirectoryOnly;

            return
            [
                .. Directory
                    .GetFiles(path, "*", searchOption)
                    .Where(f =>
                    {
                        var fileName = Path.GetFileName(f);
                        var dirName = new DirectoryInfo(
                            Path.GetDirectoryName(f) ?? string.Empty
                        ).Name;
                        return !fileName.StartsWith('.')
                            && !dirName.StartsWith('.')
                            && audioExtensions.Contains(Path.GetExtension(f).ToLower());
                    }),
            ];
        }
    }
}
