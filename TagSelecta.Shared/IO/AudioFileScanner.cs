using System.Collections.Concurrent;
using Spectre.Console;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.Shared.IO;

public class AudioFileScanner(IAnsiConsole console, ITagger tagger) : IAudioFileScanner
{
    public static HashSet<string> AllowedExtensions => [".mp3", ".flac", ".wav"];

    public List<string> Scan(IEnumerable<string> path, bool recursively)
    {
        var result = AnsiConsole.Status().Start("Searching for files...", _ => Search(path, true));
        return result;
    }

    public List<FileWithTagData> ScanAndRead(IEnumerable<string> path, CancellationToken ct)
    {
        var files = Scan(path, true);
        var result = new ConcurrentBag<FileWithTagData>();
        console
            .Progress()
            .AutoClear(true)
            .Start(ctx =>
            {
                var task = ctx.AddTask("Reading metadata...", maxValue: files.Count);
                foreach (var file in files)
                {
                    ct.ThrowIfCancellationRequested();
                    try
                    {
                        var tagData = tagger.ReadTags(file);
                        result.Add(new FileWithTagData(file, tagData));
                    }
                    catch (Exception ex)
                    {
                        console.WriteException(ex);
                    }
                    task.Increment(1);
                }
            });
        return result.ToList().OrderBy(x => x.Path).ToList();
    }

    private List<string> Search(IEnumerable<string> paths, bool recursive = false)
    {
        var files = new List<string>();
        foreach (var path in paths)
        {
            var fullPath = Path.GetFullPath(path);
            if (File.Exists(fullPath))
            {
                if (AllowedExtensions.Contains(Path.GetExtension(fullPath).ToLower()))
                {
                    files.Add(fullPath);
                }
            }
            else if (Directory.Exists(fullPath))
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
                                    && AllowedExtensions.Contains(
                                        Path.GetExtension(f).ToLowerInvariant()
                                    );
                            })
                            .Order(),
                    ]
                );
            }
        }
        return files;
    }
}
