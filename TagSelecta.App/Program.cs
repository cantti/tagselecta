using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.App.CliCommands.Find;
using TagSelecta.App.Discogs;
using TagSelecta.App.IO;
using TagSelecta.App.TagDataActions.AutoTrack;
using TagSelecta.App.TagDataActions.Discogs;
using TagSelecta.App.TagDataActions.Edit;
using TagSelecta.App.TagDataActions.ExtractPicture;
using TagSelecta.App.TagDataActions.FixAlbum;
using TagSelecta.App.TagDataActions.HelpFormatting;
using TagSelecta.App.TagDataActions.RenameFile;
using TagSelecta.App.TagDataActions.Split;
using TagSelecta.App.TagDataActions.TitleCase;
using TagSelecta.App.Tui;
using TagSelecta.Shared.Configuration;
using TagSelecta.Tagging;

namespace TagSelecta.App;

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
            config.AddCommand<TuiApp>("ui").WithDescription("Interactive UI (TUI)");
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
