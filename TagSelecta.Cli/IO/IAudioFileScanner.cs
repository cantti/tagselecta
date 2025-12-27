namespace TagSelecta.Cli.IO;

public interface IAudioFileScanner
{
    List<string> Scan(IEnumerable<string> path, bool recursively);
    List<FileWithTagData> ScanAndRead(IEnumerable<string> path);
}
