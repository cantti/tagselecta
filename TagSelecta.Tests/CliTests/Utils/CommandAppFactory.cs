using Spectre.Console.Cli;
using Spectre.Console.Testing;
using TagSelecta.DependencyInjection;

namespace TagSelecta.Tests;

public static class CommandAppFactory
{
    public static CommandAppTester CreateTestApp<TCommand>()
        where TCommand : class, ICommand
    {
        var registrar = DependencyInjectionConfig.Configure();
        var app = new CommandAppTester(registrar);
        app.Console.Interactive();
        app.SetDefaultCommand<TCommand>();
        return app;
    }
}
