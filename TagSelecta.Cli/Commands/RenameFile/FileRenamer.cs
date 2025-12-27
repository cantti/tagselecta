using TagSelecta.Tagging;

namespace TagSelecta.Cli.Commands.RenameFile;

public static class FileRenamer
{
    public static string GetNewPath(RenameFileSettings settings, FileWithTagData file)
    {
        var dir = Path.GetDirectoryName(file.Path)!;
        var formatter = new TagDataFormatter(file.TagData, file.Path);
        var newName = formatter.Format(settings.Template);
        newName = CommandHelper.CleanFileName(newName);
        newName = $"{newName}{Path.GetExtension(file.Path)}";
        var newPath = Path.Combine(dir, newName);
        return newPath;
    }
}
