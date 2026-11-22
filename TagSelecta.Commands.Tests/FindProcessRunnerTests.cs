using TagSelecta.Commands.Cli.Find;

namespace TagSelecta.Commands.Tests;

public class FindProcessRunnerTests
{
    [Fact]
    public async Task RunAsync_PassesArgumentsWithoutShellExpansion()
    {
        if (OperatingSystem.IsWindows())
        {
            return;
        }

        var outputPath = Path.GetTempFileName();
        try
        {
            var runner = new FindProcessRunner();
            var result = await runner.RunAsync(
                "/bin/sh",
                ["-c", "printf '%s\\n' \"$@\" > \"$0\"", outputPath, "a b", "", "$(not-a-command)"],
                TestContext.Current.CancellationToken
            );

            Assert.Equal(0, result);
            Assert.Equal(
                ["a b", "", "$(not-a-command)"],
                await File.ReadAllLinesAsync(outputPath, TestContext.Current.CancellationToken)
            );
        }
        finally
        {
            File.Delete(outputPath);
        }
    }

    [Fact]
    public async Task RunAsync_StopsRunningProcessWhenCancelled()
    {
        if (OperatingSystem.IsWindows())
        {
            return;
        }

        using var source = CancellationTokenSource.CreateLinkedTokenSource(
            TestContext.Current.CancellationToken
        );
        source.CancelAfter(TimeSpan.FromMilliseconds(200));
        var task = new FindProcessRunner().RunAsync("/bin/sleep", ["30"], source.Token);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            await task.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken)
        );
    }
}
