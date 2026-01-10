using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Cli.CliCommands.AutoTrack;
using TagSelecta.Cli.CliCommands.Discogs;
using TagSelecta.Cli.CliCommands.ExtractPicture;
using TagSelecta.Cli.CliCommands.Find;
using TagSelecta.Cli.CliCommands.FixAlbum;
using TagSelecta.Cli.CliCommands.HelpFormatting;
using TagSelecta.Cli.CliCommands.RenameFile;
using TagSelecta.Cli.CliCommands.Set;
using TagSelecta.Cli.CliCommands.Split;
using TagSelecta.Cli.CliCommands.TitleCase;
using TagSelecta.Cli.Discogs;
using TagSelecta.Cli.IO;
using TagSelecta.Cli.Tui;
using TagSelecta.Shared.Configuration;
using TagSelecta.Tagging;

namespace TagSelecta.Cli;

class Program
{
    static int Main(string[] args)
    {
        var cst = new CancellationTokenSource();
        SetAnsiSupport();
        ConfigureCancel(cst);
        var services = ConfigureServices();
        var app = new CommandApp(new TypeRegistrar(services));
        app.Configure(config =>
        {
            config.PropagateExceptions();
            config.AddCommand<TuiApp>("edit").WithAlias("e");
            AddCommands(config, services);
            config.SetApplicationVersion(GetAppVersion());
        });
        return app.Run(args, cst.Token);
    }

    private static string GetAppVersion()
    {
        return Assembly
                .GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion
            ?? "unknown";
    }

    private static void SetAnsiSupport()
    {
        var noAnsi = Environment.GetEnvironmentVariable("TAGSELECTA_NOANSI") == "1";
        AnsiConsole.Profile.Capabilities.Ansi = !noAnsi;
    }

    private static void AddCommands(IConfigurator config, IServiceCollection services)
    {
        config.AddEdit(services);
        config.AddExtractPicture(services);
        config.AddTitleCase(services);
        config.AddSplit(services);
        config.AddDiscogs(services);
        config.AddAutoTrack(services);
        config.AddFixAlbum(services);
        config.AddRenameFile(services);
        config.AddFind(services);
        config.AddHelpFormatting(services);
    }

    private static ServiceCollection ConfigureServices()
    {
        var services = new ServiceCollection();
        services.AddDiscogs();
        services.AddCommonTagDataServices();
        services.AddTransient<IConfig, Config>();
        services.AddTransient<IFileSystem, FileSystem>();
        services.AddTransient<ITagger, Tagger>();
        services.AddTransient<IAudioFileScanner, AudioFileScanner>();

        // todo refactor that action
        // services.AddTransient<FileAction<RenameDirSettings>, RenameDirAction>();
        return services;
    }

    private static void ConfigureCancel(CancellationTokenSource cst)
    {
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            cst.Cancel();
            Console.WriteLine("Cancellation requested...");
        };
    }
}
