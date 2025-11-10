using Spectre.Console;
using TagSelecta.Tagging;

namespace TagSelecta.Commands.FileCommands;

public class ReadSettings : BaseSettings { }

public class ReadAction(IAnsiConsole console) : FileAction<ReadSettings>
{
    private bool _allConfirmed;

    protected override void ProcessFile(FileActionContext<ReadSettings> context)
    {
        TagData tagData;
        try
        {
            tagData = Tagger.ReadTags(context.CurrentFile);
        }
        catch (Exception ex)
        {
            console.WriteLine(ex.Message);
            return;
        }
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
