namespace TagSelecta.Cli.Commands.RenameFile;

public class RenameFileOperation
{
    public required string Path { get; init; }
    public required string NewPath { get; set; }
    public bool IsSaved { get; set; }
}
