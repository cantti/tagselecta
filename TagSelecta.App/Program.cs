using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Shared.Configuration;
using TagSelecta.Shared.IO;
using TagSelecta.Shared.TagDataActions;
using TagSelecta.Shared.Tagging;
using TagSelecta.Tui;

namespace TagSelecta.App;

class Program
{
    static int Main(string[] args)
    {
        var cst = new CancellationTokenSource();
        SetAnsiSupport();
        ConfigureCancel(cst);

        var services = new ServiceCollection();

        services.AddTuiServices();

        AddCommonServices(services);

        var app = new CommandApp(new TypeRegistrar(services));
        app.Configure(config =>
        {
            config.PropagateExceptions();

            // add tui command
            config.AddCommand<TuiApp>("ui").WithDescription("Interactive UI (TUI)");

            // add tag data actions and commands
            AddTagDataActions(config, services);

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

    private static void AddTagDataActions(IConfigurator config, IServiceCollection services)
    {
        config.AddEdit(services);
        config.AddExtractPicture(services);
        config.AddTitleCase(services);
        config.AddSplit(services);
        config.AddDiscogs(services);
        config.AddAutoTrack(services);
        config.AddMove(services);
        config.AddFind(services);
    }

    private static void AddCommonServices(IServiceCollection services)
    {
        services.AddTransient<IConfig, Config>();
        services.AddTransient<IFileSystem, FileSystem>();
        services.AddTransient<ITagger, Tagger>();
        services.AddTransient<IAudioFileScanner, AudioFileScanner>();
        services.AddTransient<ITagDataOperationWriter, TagDataOperationWriter>();
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
