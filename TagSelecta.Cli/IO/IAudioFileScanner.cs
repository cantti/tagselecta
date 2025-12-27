namespace TagSelecta.Cli.IO;

public interface IAudioFileScanner
{
    List<string> Scan(IEnumerable<string> paths, bool recursive = false);
}
