using TagSelecta.Tagging;

namespace TagSelecta.App.CliCommands.RenameFile;

public static class FileRenamer
{
    public static string GetNewPath(RenameFileSettings settings, IFileContext file)
    {
        var dir = Path.GetDirectoryName(file.CurrentPath)!;
        var formatter = new TagDataFormatter(file.CurrentTagData, file.CurrentPath);
        var newName = formatter.Format(settings.Template);
        newName = CommandHelper.CleanFileName(newName);
        newName = $"{newName}{Path.GetExtension(file.CurrentPath)}";
        var newPath = Path.Combine(dir, newName);
        return newPath;
    }
}
