using Spectre.Console;
using TagSelecta.Tagging;

namespace TagSelecta.Cli.Commands.FileCommands;

public class ReadSettings : BaseSettings { }

public class ReadAction(IAnsiConsole console) : FileAction<ReadSettings>
{
    private bool _allConfirmed;

    protected override void ProcessFile(FileActionContext<ReadSettings> context)
    {
        var tagData = Tagger.ReadTags(context.CurrentFile);

        TagDataPrinter.PrintTagData(console, tagData);

        if (context.CurrentFileIndex < context.Files.Count - 1)
        {
            if (!_allConfirmed)
            {
                var confirmation = console.Prompt(
                    new TextPrompt<string>("Show next? ([y]es/[a]ll)".EscapeMarkup())
                        .AddChoices(["y", "a"])
                        .DefaultValue("y")
                );

                if (confirmation == "a")
                {
                    _allConfirmed = true;
                }
            }
        }
        return;
    }
}
