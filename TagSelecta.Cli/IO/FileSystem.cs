namespace TagSelecta.Cli.IO;

public class FileSystem : IFileSystem
{
    public void Move(string sourceFileName, string destFileName)
    {
        File.Move(sourceFileName, destFileName);
    }
}
