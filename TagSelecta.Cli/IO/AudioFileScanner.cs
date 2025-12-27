namespace TagSelecta.Cli.IO;

public class AudioFileScanner : IAudioFileScanner
{
    private static readonly HashSet<string> allowedExtensions = [".mp3", ".flac", ".wav"];

    public List<string> Scan(IEnumerable<string> paths, bool recursive = false)
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
                                    && allowedExtensions.Contains(Path.GetExtension(f).ToLower());
                            })
                            .Order(),
                    ]
                );
            }
        }
        return files;
    }
}
