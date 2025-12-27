using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace TagSelecta.Cli.Commands.Write;

public static class Registration
{
    public static void AddWrite(this IConfigurator configurator, IServiceCollection services)
    {
        configurator
            .AddTagDataAction<WriteAction>(services, "write")
            .WithDescription(
                "Read and write tags. Do not specify any options for reading. Unknown options become custom tags."
            )
            // Basic examples
            .WithExample(
                [
                    "write",
                    "song.mp3",
                    "-t",
                    "'Song 1'",
                    "-a",
                    "'Artist1;Artist 2'",
                    "--some-custom-tag",
                    "custom-value",
                ]
            )
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
