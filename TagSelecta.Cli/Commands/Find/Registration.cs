using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace TagSelecta.Cli.Commands.Find;

public static class Registration
{
    public static void AddFind(this IConfigurator configurator, IServiceCollection services)
    {
        configurator
            .AddCommand<FindCommand>("find")
            .WithDescription("Find files by metadata")
            .WithExample(
                ["find", ".", "-q", "\"title | string.downcase |  string.contains 'dub'\""]
            );
    }
}
