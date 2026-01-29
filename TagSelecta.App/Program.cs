using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.App.Config;
using TagSelecta.Commands;
using TagSelecta.Shared;

namespace TagSelecta.App;

class Program
{
    static int Main(string[] args)
    {
        var cst = new CancellationTokenSource();
        SetAnsiSupport();
        ConfigureCancel(cst);

        var services = new ServiceCollection();

        var appConfig = AppConfigReader.Read();
        services.AddSettings(appConfig);

        services.AddCommandServices().AddSharedServices();

        var app = new CommandApp(new TypeRegistrar(services));
        app.Configure(config =>
        {
            if (appConfig.General.Debug)
            {
                config.PropagateExceptions();
            }
            config.AddCommands(services);
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
