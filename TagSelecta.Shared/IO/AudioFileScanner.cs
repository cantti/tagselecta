using System.Collections.Concurrent;
using Spectre.Console;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.Shared.IO;

public class AudioFileScanner(IAnsiConsole console, ITagger tagger) : IAudioFileScanner
{
    public static HashSet<string> AllowedExtensions => [".mp3", ".flac", ".wav", ".ogg"];

    public List<FileWithTagData> SearchAndRead(
        IEnumerable<string> path,
        bool recursive,
        CancellationToken ct
    )
    {
        var files = AnsiConsole
            .Status()
            .Start("Searching for files...", _ => Search(path, recursive));
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

    public List<string> Search(IEnumerable<string> paths, bool recursive)
    {
        var files = new List<string>();
        foreach (var path in paths)
        {
            var fullPath = PathUtils.ExpandToFullPath(path);
            if (File.Exists(fullPath))
            {
                if (AllowedExtensions.Contains(PathUtils.GetExtension(fullPath).ToLower()))
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
                                var fileName = PathUtils.GetFileName(f);
                                var dirName = new DirectoryInfo(
                                    PathUtils.GetDirectoryName(f) ?? string.Empty
                                ).Name;
                                return !fileName.StartsWith('.')
                                    && !dirName.StartsWith('.')
                                    && AllowedExtensions.Contains(
                                        PathUtils.GetExtension(f).ToLowerInvariant()
                                    );
                            })
                            .Order(),
                    ]
                );
            }
        }

        files.Sort();
        return files;
    }
}
