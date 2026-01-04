namespace TagSelecta.Cli.Commands.TagDataCommandShared.BulkWrite;

public interface IBulkWriter
{
    void WriteAll(List<TagDataOperation> operations);
}
