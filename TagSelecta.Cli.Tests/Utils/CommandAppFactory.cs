using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace TagSelecta.Cli.Tests.Utils;

public static class CommandAppFactory
{
    public static CommandAppTester CreateTestApp<TCommand>()
        where TCommand : class, ICommand
    {
        var registrar = new TypeRegistrar(Commands.DependencyInjection.Configure());
        var app = new CommandAppTester(registrar);
        app.Console.Interactive();
        app.SetDefaultCommand<TCommand>();
        return app;
    }
}
