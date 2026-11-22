using System.ComponentModel;
using System.Diagnostics;

namespace TagSelecta.Commands.Cli.Find;

public class FindProcessRunner : IFindProcessRunner
{
    public async Task<int> RunAsync(
        string program,
        IReadOnlyList<string> arguments,
        CancellationToken token
    )
    {
        token.ThrowIfCancellationRequested();
        var startInfo = new ProcessStartInfo(program) { UseShellExecute = false };
        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process =
            Process.Start(startInfo)
            ?? throw new InvalidOperationException("Process did not start.");
        try
        {
            await process.WaitForExitAsync(token);
            return process.ExitCode;
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
            try
            {
                process.Kill(entireProcessTree: true);
            }
            catch (InvalidOperationException)
            {
                // process already exited
            }
            catch (Win32Exception) when (process.HasExited)
            {
                // process already exited
            }

            await process.WaitForExitAsync(CancellationToken.None);
            throw;
        }
    }
}
