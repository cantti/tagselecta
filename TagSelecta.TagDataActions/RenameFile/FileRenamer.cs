using System.Text.RegularExpressions;
using TagSelecta.Shared.TagDataActions;
using TagSelecta.Tagging;

namespace TagSelecta.TagDataActions.RenameFile;

public static class FileRenamer
{
    public static string GetNewPath(RenameFileSettings settings, IFileContext file)
    {
        var dir = Path.GetDirectoryName(file.CurrentPath)!;
        var formatter = new TagDataFormatter(file.CurrentTagData, file.CurrentPath);
        var newName = formatter.Format(settings.Template);
        newName = CleanFileName(newName);
        newName = $"{newName}{Path.GetExtension(file.CurrentPath)}";
        var newPath = Path.Combine(dir, newName);
        return newPath;
    }

    private static string CleanFileName(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        input = input
            .Replace(Path.DirectorySeparatorChar.ToString(), "")
            .Replace(Path.AltDirectorySeparatorChar.ToString(), "");
        input = Regex.Replace(input, @"\s+", " ");
        return input;
    }
}
