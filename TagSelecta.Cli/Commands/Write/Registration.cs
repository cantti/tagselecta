using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace TagSelecta.Cli.Commands.Write;

public static class Registration
{
    public static void AddWrite(this IConfigurator configurator, IServiceCollection services)
    {
        configurator
            .AddTagDataAction<WriteAction>(services, "write")
            .WithDescription("Write tags.")
            // Basic examples
            .WithExample(["write", "song.mp3", "-t", "'Song 1'", "-a", "'Artist1;Artist 2'"])
            .WithExample(["write", "song.mp3", "-c", "'url=https://github.com'"])
            .WithExample(
                [
                    "write",
                    "song.mp3",
                    "-a",
                    "'{{ artist | regex.replace \"^VA$\" \"Various Artists\" \"-i\" }}'",
                ]
            );
    }
}
