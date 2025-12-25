namespace TagSelecta.Shared.IO;

public interface IFileSystem
{
    void Move(string sourceFileName, string destFileName);
}
