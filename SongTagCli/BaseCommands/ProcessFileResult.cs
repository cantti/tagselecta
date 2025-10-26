namespace SongTagCli.BaseCommands;

public class ProcessFileResult
{
    public ProcessFileResult(Exception ex)
    {
        Exception = ex;
        Status = ProcessFileResultStatus.Error;
    }

    public ProcessFileResult(ProcessFileResultStatus status, string? message = null)
    {
        Status = status;
        Message = message;
    }

    public ProcessFileResultStatus Status { get; init; }
    public string? Message { get; init; }
    public Exception? Exception { get; init; }
}
