namespace TagSelecta.Shared.IO;

public interface IAudioFileScanner
{
    List<string> Scan(IEnumerable<string> path, bool recursively);
    List<FileWithTagData> ScanAndRead(IEnumerable<string> path, CancellationToken ct);
}
