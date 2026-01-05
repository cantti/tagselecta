using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Cli.Commands.AutoTrack;
using TagSelecta.Cli.Commands.Discogs;
using TagSelecta.Cli.Commands.Edit;
using TagSelecta.Cli.Commands.ExtractPicture;
using TagSelecta.Cli.Commands.Find;
using TagSelecta.Cli.Commands.FixAlbum;
using TagSelecta.Cli.Commands.HelpFormatting;
using TagSelecta.Cli.Commands.RenameFile;
using TagSelecta.Cli.Commands.Split;
using TagSelecta.Cli.Commands.TitleCase;
using TagSelecta.Cli.Commands.Tui;
using TagSelecta.Cli.Discogs;
using TagSelecta.Cli.IO;
using TagSelecta.Shared.Configuration;
using TagSelecta.Tagging;

namespace TagSelecta.Cli;

class Program
{
    static int Main(string[] args)
    {
        SetAnsiSupport();
        ConfigureCancel();
        var services = ConfigureServices();
        var app = new CommandApp(new TypeRegistrar(services));
        app.Configure(config =>
        {
            config.AddCommand<TuiCommand>("tui");
            AddCommands(config, services);
            config.SetApplicationVersion(GetAppVersion());
        });
        try
        {
            return app.Run(args);
        }
        finally
        {
            AltScreen.Exit();
        }
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

    private static void ConfigureCancel()
    {
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            AltScreen.Exit();
            Environment.Exit(130);
        };
    }
}
