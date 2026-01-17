namespace TagSelecta.Shared.IO;

public class FileSystem : IFileSystem
{
    public void Move(string sourceFileName, string destFileName)
    {
        File.Move(sourceFileName, destFileName);
    }

    public void CreateDirectory(string getDirectoryName)
    {
        Directory.CreateDirectory(getDirectoryName);
    }

    public bool Exists(string path)
    {
        return Path.Exists(path);
    }

    public bool IsDirectoryEmpty(string path)
    {
        return !Directory.EnumerateFileSystemEntries(path).Any();
    }

    public void DeleteDirectory(string path)
    {
        Directory.Delete(path);
    }

    public string[] GetFiles(string path)
    {
        return Directory.GetFiles(path);
    }

    public void Copy(string sourceFileName, string destFileName)
    {
        File.Copy(sourceFileName, destFileName);
    }
}
