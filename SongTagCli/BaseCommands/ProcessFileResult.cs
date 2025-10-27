namespace SongTagCli.BaseCommands;

public class ProcessFileResult
{
    public ProcessFileResult(ProcessFileResultStatus status, string? message = null)
    {
        Status = status;
        Message = message;
    }

    public ProcessFileResult(Exception ex)
    {
        Exception = ex;
        Status = ProcessFileResultStatus.Error;
    }

    public static ProcessFileResult Success(string? message = "")
    {
        return new ProcessFileResult(ProcessFileResultStatus.Success, message);
    }

    public static ProcessFileResult Skipped(string? message = "Skipped.")
    {
        return new ProcessFileResult(ProcessFileResultStatus.Skipped, message);
    }

    public static ProcessFileResult Error(string? message)
    {
        return new ProcessFileResult(ProcessFileResultStatus.Error, message);
    }

    public ProcessFileResultStatus Status { get; init; }
    public string? Message { get; init; }
    public Exception? Exception { get; init; }
}
