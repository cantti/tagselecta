namespace TagSelecta.Cli.IO;

public interface IFileSystem
{
    void Move(string sourceFileName, string destFileName);
}
