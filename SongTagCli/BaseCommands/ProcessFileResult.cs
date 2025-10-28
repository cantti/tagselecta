namespace SongTagCli.BaseCommands;

public class ProcessFileResult
{
    public ProcessFileResult(ResultStatus status, string? message = null)
    {
        Status = status;
        Message = message;
    }

    public ProcessFileResult(Exception ex)
    {
        Exception = ex;
        Status = ResultStatus.Error;
    }

    public static ProcessFileResult Success(string? message = "")
    {
        return new ProcessFileResult(ResultStatus.Success, message);
    }

    public static ProcessFileResult Skipped(string? message = "Skipped.")
    {
        return new ProcessFileResult(ResultStatus.Skipped, message);
    }

    public static ProcessFileResult Error(string? message)
    {
        return new ProcessFileResult(ResultStatus.Error, message);
    }

    public ResultStatus Status { get; init; }
    public string? Message { get; init; }
    public Exception? Exception { get; init; }
}
