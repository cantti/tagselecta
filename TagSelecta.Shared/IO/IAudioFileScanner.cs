namespace TagSelecta.Shared.IO;

public interface IAudioFileScanner
{
    List<FileWithTagData> SearchAndRead(IEnumerable<string> path, CancellationToken ct);
    List<string> Search(IEnumerable<string> paths, bool recursive = false);
}
