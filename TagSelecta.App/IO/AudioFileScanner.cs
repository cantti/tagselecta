using System.Collections.Concurrent;
using Spectre.Console;
using TagSelecta.Tagging;

namespace TagSelecta.App.IO;

public class AudioFileScanner(IAnsiConsole console, ITagger tagger) : IAudioFileScanner
{
    private static readonly HashSet<string> AllowedExtensions = [".mp3", ".flac", ".wav"];

    public List<string> Scan(IEnumerable<string> path, bool recursively)
    {
        var result = AnsiConsole.Status().Start("Searching for files...", _ => Search(path, true));
        return result;
    }

    public List<FileWithTagData> ScanAndRead(IEnumerable<string> path)
    {
        var files = Scan(path, true);
        var result = new ConcurrentBag<FileWithTagData>();
        console
            .Progress()
            .AutoClear(true)
            .Start(ctx =>
            {
                var progressLock = new object();
                var consoleLock = new object();
                var task = ctx.AddTask("Reading metadata...", maxValue: files.Count);
                Parallel.ForEach(
                    files,
                    file =>
                    {
                        try
                        {
                            var tagData = tagger.ReadTags(file);
                            result.Add(new FileWithTagData(file, tagData));
                        }
                        catch (Exception ex)
                        {
                            lock (consoleLock)
                            {
                                console.WriteException(ex);
                            }
                        }
                        lock (progressLock)
                        {
                            task.Increment(1);
                        }
                    }
                );
            });
        return result.ToList().OrderBy(x => x.Path).ToList();
    }

    // public List<FileWithTagData> ScanAndRead(IEnumerable<string> path)
    // {
    //     var files = Scan(path, true);
    //     var result = console
    //         .Progress()
    //         .Start(ctx =>
    //         {
    //             var task = ctx.AddTask("Reading metadata...", maxValue: files.Count);
    //             var result = new List<FileWithTagData>();
    //             for (var i = 0; i < files.Count; i++)
    //             {
    //                 var file = files[i];
    //                 try
    //                 {
    //                     var tagData = tagger.ReadTags(file);
    //                     result.Add(new FileWithTagData { Path = file, TagData = tagData });
    //                 }
    //                 catch (Exception ex)
    //                 {
    //                     console.WriteException(ex);
    //                 }
    //
    //                 task.Description = $"Reading metadata for {i + 1} of {files.Count}";
    //
    //                 task.Increment(1);
    //             }
    //
    //             return result;
    //         });
    //     return result;
    // }

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
