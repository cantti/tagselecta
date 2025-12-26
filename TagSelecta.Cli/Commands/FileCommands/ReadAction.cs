using Spectre.Console;
using TagSelecta.Tagging;

namespace TagSelecta.Cli.Commands.FileCommands;

public class ReadSettings : BaseSettings { }

public class ReadAction(IAnsiConsole console, ITagger tagger) : FileAction<ReadSettings>
{
    private bool _allConfirmed;

    protected override void ProcessFile(IFileActionContext<ReadSettings> context)
    {
        var tagData = tagger.ReadTags(context.CurrentFile);

        TagDataPrinter.PrintTagData(console, tagData);

        // if (context.CurrentFileIndex < context.Files.Count - 1)
        // {
        //     if (!_allConfirmed)
        //     {
        //         var confirmation = console.Prompt(
        //             new TextPrompt<string>("Show next? ([y]es/[a]ll)".EscapeMarkup())
        //                 .AddChoices(["y", "a"])
        //                 .DefaultValue("y")
        //         );
        //
        //         if (confirmation == "a")
        //         {
        //             _allConfirmed = true;
        //         }
        //     }
        // }
        return;
    }
}
