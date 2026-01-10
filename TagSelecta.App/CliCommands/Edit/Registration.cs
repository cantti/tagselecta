using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using TagSelecta.App.Tui;

namespace TagSelecta.App.CliCommands.Edit;

public static class Registration
{
    public static void AddEdit(this IConfigurator configurator, IServiceCollection services)
    {
        // configurator
        //     .AddTagDataAction<EditAction>(services, "edit")
        //     .WithAlias("e")
        //     .WithDescription(
        //         "Edit (read and write) tags. Unrecognized options are saved as custom fields. Another way to edit custom fields is to use --custom option."
        //     )
        //     // Basic examples
        //     .WithExample(
        //         [
        //             "edit",
        //             "song.mp3",
        //             "-t",
        //             "'Song 1'",
        //             "-a",
        //             "'Artist1;Artist 2'",
        //             "-s",
        //             "description=test",
        //         ]
        //     )
        //     .WithExample(["edit", "song.mp3", "-c", "'url=https://github.com'"])
        //     .WithExample(
        //         [
        //             "edit",
        //             "song.mp3",
        //             "-a",
        //             "'{{ artist | regex.replace \"^VA$\" \"Various Artists\" \"-i\" }}'",
        //         ]
        //     );
        services.AddTransient<ITagDataAction, EditAction>();
        services.AddTransient<EditAction>();
    }
}
