using Spectre.Console;
using Spectre.Console.Cli;

namespace TagSelecta.BaseCommands;

// provides settings and files
public abstract class BaseCommand<TSettings>(IAnsiConsole console) : AsyncCommand<TSettings>
    where TSettings : BaseSettings
{
    protected IAnsiConsole Console { get; } = console;

    private TSettings? _settings;
    protected TSettings Settings =>
        _settings ?? throw new InvalidOperationException("_settings not set");

    protected List<string> Files { get; private set; } = [];

    public override async Task<int> ExecuteAsync(
        CommandContext context,
        TSettings settings,
        CancellationToken ct
    )
    {
        _settings = settings;

        Console.MarkupLine("Searching for files...");

        Console.WriteLine();

        Files = FileHelper.GetAllAudioFiles(settings.Path, true);

        Console.MarkupLineInterpolated(
            $"[yellow]{Files.Count}[/] {(Files.Count == 1 ? "file" : "files")} found."
        );

        Console.WriteLine();

        await ProcessAsync();

        return 0;
    }

    protected virtual void Process() { }

    protected virtual Task ProcessAsync()
    {
        Process();
        return Task.CompletedTask;
    }

    protected void PrintCurrentFile(string file, int index, int total)
    {
        Console.MarkupInterpolated($"[dim]>[/] [yellow]({index + 1}/{total})[/] \"");
        var path = new TextPath(file)
            .RootColor(Color.White)
            .SeparatorColor(Color.White)
            .StemColor(Color.White)
            .LeafColor(Color.Yellow);
        Console.Write(path);
        Console.Write("\"");
        Console.WriteLine();
    }
}
