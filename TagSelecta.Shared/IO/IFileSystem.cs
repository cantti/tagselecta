namespace TagSelecta.Shared.IO;

public interface IFileSystem
{
    void Move(string sourceFileName, string destFileName);
    void CreateDirectory(string getDirectoryName);
    bool Exists(string path);
    bool IsDirectoryEmpty(string path);
    void DeleteDirectory(string path);
    string[] GetFiles(string path);
    void Copy(string sourceFileName, string destFileName);
}
