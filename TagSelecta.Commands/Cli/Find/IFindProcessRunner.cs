namespace TagSelecta.Commands.Cli.Find;

public interface IFindProcessRunner
{
    Task<int> RunAsync(string program, IReadOnlyList<string> arguments, CancellationToken token);
}
