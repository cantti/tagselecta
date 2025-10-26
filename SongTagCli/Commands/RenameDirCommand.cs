using SongTagCli.BaseCommands;
using SongTagCli.Misc;
using SongTagCli.Tagging;
using Spectre.Console;

namespace SongTagCli.Commands;

public class RenameDirSettings : FileProcessingSettings { }

public class RenameDirCommand(IAnsiConsole console)
    : FileProcessingCommandBase<RenameDirSettings>(console)
{
    protected override bool PrintFileAfterProcessing => false;

    private readonly List<string> _renamed = [];

    protected override async Task<ProcessFileResult> ProcessFileAsync(
        StatusContext ctx,
        RenameDirSettings settings,
        List<string> files,
        string file
    )
    {
        var dir = Path.GetDirectoryName(file)!;
        if (_renamed.Contains(dir))
        {
            return new ProcessFileResult(ProcessFileResultStatus.Skipped);
        }
        _renamed.Add(dir);
        var tagData = Tagger.ReadTags(file);
        var newName = $"{tagData.Year} - {tagData.AlbumArtist.Print()} - {tagData.Album}";
        RenameDirectory(dir, newName);
        return await Task.FromResult(new ProcessFileResult(ProcessFileResultStatus.Success));
    }

    private void RenameDirectory(string dirPath, string newName)
    {
        if (!Directory.Exists(dirPath))
            throw new DirectoryNotFoundException($"Source directory not found: {dirPath}");

        string parentDir = Path.GetDirectoryName(dirPath)!;
        string newPath = Path.Combine(parentDir, newName);
        Console.MarkupLine($"Directory rename details:");
        Console.MarkupLine("  Old: {0}", dirPath);
        Console.MarkupLine("  New: {0}", newPath);

        if (Directory.Exists(newPath))
            throw new SongTagCliException($"Target directory already exists: {newPath}");

        Directory.Move(dirPath, newPath);
    }
}
