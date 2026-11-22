using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.App;
using TagSelecta.Commands.Cli.Find;
using TagSelecta.Shared.IO;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.Commands.Tests;

public class FindCommandTests
{
    [Fact]
    public async Task Find_WithoutExecPrintsMatchingPaths()
    {
        var scanner = new TestScanner();
        var runner = new TestRunner();
        using var writer = new StringWriter();
        var app = CreateApp(scanner, runner, writer);

        var result = await app.RunAsync(
            ["find", "music", "--query", "{{ genre == 'Reggae' }}"],
            TestContext.Current.CancellationToken
        );

        Assert.Equal(0, result);
        Assert.Contains(TestScanner.FirstPath, writer.ToString());
        Assert.DoesNotContain(TestScanner.OtherPath, writer.ToString());
        Assert.Empty(runner.Calls);
    }

    [Fact]
    public async Task Find_ExecPassesArgumentsWithoutShellAndOnlyForMatches()
    {
        var scanner = new TestScanner();
        var runner = new TestRunner();
        using var writer = new StringWriter();
        var app = CreateApp(scanner, runner, writer);

        var result = await app.RunAsync(
            [
                "find",
                "music",
                "--query",
                "{{ genre == 'Reggae' }}",
                "--exec",
                "tool",
                "--",
                "--input={}",
                "{}",
                "--flag",
                "two words",
                "",
                "--",
                "'quoted'",
            ],
            TestContext.Current.CancellationToken
        );

        Assert.Equal(0, result);
        Assert.Equal(["music"], scanner.Paths);
        Assert.Equal(2, runner.Calls.Count);
        Assert.Equal("tool", runner.Calls[0].Program);
        Assert.Equal(
            [
                $"--input={TestScanner.FirstPath}",
                TestScanner.FirstPath,
                "--flag",
                "two words",
                "",
                "--",
                "'quoted'",
            ],
            runner.Calls[0].Arguments
        );
        Assert.Equal($"--input={TestScanner.SecondPath}", runner.Calls[1].Arguments[0]);
        Assert.DoesNotContain(TestScanner.FirstPath, writer.ToString());
    }

    [Fact]
    public async Task Find_ExecAppendsPathWhenPlaceholderIsAbsent()
    {
        var runner = new TestRunner();
        using var writer = new StringWriter();
        var app = CreateApp(new TestScanner(), runner, writer);

        var result = await app.RunAsync(
            ["find", "music", "--exec", "tool", "--", "--verbose"],
            TestContext.Current.CancellationToken
        );

        Assert.Equal(0, result);
        Assert.Equal(["--verbose", TestScanner.FirstPath], runner.Calls[0].Arguments);
    }

    [Fact]
    public async Task Find_ExecWithoutArgumentsAppendsPath()
    {
        var runner = new TestRunner();
        using var writer = new StringWriter();
        var app = CreateApp(new TestScanner(), runner, writer);

        var result = await app.RunAsync(
            ["find", "music", "--exec", "tool"],
            TestContext.Current.CancellationToken
        );

        Assert.Equal(0, result);
        Assert.Equal([TestScanner.FirstPath], runner.Calls[0].Arguments);
    }

    [Fact]
    public async Task Find_ExecDoesNotRunWhenNothingMatches()
    {
        var runner = new TestRunner();
        using var writer = new StringWriter();
        var app = CreateApp(new TestScanner(), runner, writer);

        var result = await app.RunAsync(
            ["find", "music", "--query", "{{ false }}", "--exec", "tool"],
            TestContext.Current.CancellationToken
        );

        Assert.Equal(0, result);
        Assert.Empty(runner.Calls);
        Assert.Equal("", writer.ToString());
    }

    [Fact]
    public async Task Find_ExecStopsAndReturnsChildFailure()
    {
        var runner = new TestRunner { ExitCode = 17 };
        using var writer = new StringWriter();
        var app = CreateApp(new TestScanner(), runner, writer);

        var result = await app.RunAsync(
            ["find", "music", "--exec", "tool"],
            TestContext.Current.CancellationToken
        );

        Assert.Equal(17, result);
        Assert.Single(runner.Calls);
        Assert.Contains("exit code 17", writer.ToString());
    }

    [Fact]
    public async Task Find_ExecReportsStartFailure()
    {
        var runner = new TestRunner { Error = new Win32Exception("not found") };
        using var writer = new StringWriter();
        var app = CreateApp(new TestScanner(), runner, writer);

        var result = await app.RunAsync(
            ["find", "music", "--exec", "missing"],
            TestContext.Current.CancellationToken
        );

        Assert.Equal(1, result);
        Assert.Single(runner.Calls);
        Assert.Contains("Unable to run 'missing'", writer.ToString());
    }

    [Fact]
    public async Task Find_ExecRejectsUnknownFindOptionsBeforeRunning()
    {
        var scanner = new TestScanner();
        var runner = new TestRunner();
        using var writer = new StringWriter();
        var app = CreateApp(scanner, runner, writer);

        var result = await app.RunAsync(
            ["find", "music", "--qurey", "{{ false }}", "--exec", "tool", "--", "{}"],
            TestContext.Current.CancellationToken
        );

        Assert.Equal(1, result);
        Assert.Contains("Unknown find option: --qurey", writer.ToString());
        Assert.Empty(scanner.Paths);
        Assert.Empty(runner.Calls);
    }

    [Fact]
    public async Task Find_ExecAllowsFindOptionNamesAfterSeparator()
    {
        var runner = new TestRunner();
        using var writer = new StringWriter();
        var app = CreateApp(new TestScanner(), runner, writer);

        var result = await app.RunAsync(
            ["find", "music", "--query", "{{ true }}", "--exec", "tool", "--", "--query", "{}"],
            TestContext.Current.CancellationToken
        );

        Assert.Equal(0, result);
        Assert.Equal(["--query", TestScanner.FirstPath], runner.Calls[0].Arguments);
    }

    [Fact]
    public async Task Find_RejectsArgumentsAfterSeparatorWithoutExec()
    {
        var runner = new TestRunner();
        using var writer = new StringWriter();
        var app = CreateApp(new TestScanner(), runner, writer);

        var result = await app.RunAsync(
            ["find", "music", "--", "unexpected"],
            TestContext.Current.CancellationToken
        );

        Assert.Equal(1, result);
        Assert.Contains("require --exec", writer.ToString());
        Assert.Empty(runner.Calls);
    }

    [Fact]
    public async Task Find_StopsWhenCancelled()
    {
        using var source = CancellationTokenSource.CreateLinkedTokenSource(
            TestContext.Current.CancellationToken
        );
        var runner = new TestRunner { OnRun = source.Cancel };
        using var writer = new StringWriter();
        var app = CreateApp(new TestScanner(), runner, writer);

        var result = await app.RunAsync(["find", "music", "--exec", "tool"], source.Token);

        Assert.Equal(130, result);
        Assert.Single(runner.Calls);
    }

    private static CommandApp CreateApp(TestScanner scanner, TestRunner runner, StringWriter writer)
    {
        var console = AnsiConsole.Create(
            new AnsiConsoleSettings { Ansi = AnsiSupport.No, Out = new AnsiConsoleOutput(writer) }
        );
        var services = new ServiceCollection();
        services.AddSingleton<IAnsiConsole>(console);
        services.AddSingleton<IAudioFileScanner>(scanner);
        services.AddSingleton<IFindProcessRunner>(runner);
        var app = new CommandApp(new TypeRegistrar(services));
        app.Configure(config =>
        {
            config.ConfigureConsole(console);
            config.AddCommand<FindCommand>("find");
        });
        return app;
    }

    private class TestScanner : IAudioFileScanner
    {
        public const string FirstPath = "/music/first song.mp3";
        public const string SecondPath = "/music/[album] $demo.mp3";
        public const string OtherPath = "/music/other.mp3";
        public List<string> Paths { get; private set; } = [];

        public List<FileWithTagData> SearchAndRead(
            IEnumerable<string> path,
            bool recursive,
            CancellationToken ct
        )
        {
            Paths = path.ToList();
            return [File(FirstPath, "Reggae"), File(SecondPath, "Reggae"), File(OtherPath, "Rock")];
        }

        public List<string> Search(IEnumerable<string> paths, bool recursive)
        {
            return [];
        }

        private static FileWithTagData File(string path, string genre)
        {
            var tagData = new TagData();
            tagData.SetValue("genre", genre);
            return new FileWithTagData(path, tagData);
        }
    }

    private class TestRunner : IFindProcessRunner
    {
        public List<(string Program, IReadOnlyList<string> Arguments)> Calls { get; } = [];
        public int ExitCode { get; set; }
        public Exception? Error { get; set; }
        public Action? OnRun { get; set; }

        public Task<int> RunAsync(
            string program,
            IReadOnlyList<string> arguments,
            CancellationToken token
        )
        {
            Calls.Add((program, arguments));
            OnRun?.Invoke();
            token.ThrowIfCancellationRequested();
            if (Error is not null)
            {
                throw Error;
            }

            return Task.FromResult(ExitCode);
        }
    }
}
