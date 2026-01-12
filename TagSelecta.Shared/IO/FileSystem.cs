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
}
