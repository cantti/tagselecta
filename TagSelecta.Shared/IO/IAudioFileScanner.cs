namespace TagSelecta.Shared.IO;

public interface IAudioFileScanner
{
    List<FileWithTagData> SearchAndRead(
        IEnumerable<string> path,
        bool recursive,
        CancellationToken ct
    );
    List<string> Search(IEnumerable<string> paths, bool recursive);
}
