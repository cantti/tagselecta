using Spectre.Console;
using TagSelecta.BaseCommands;
using TagSelecta.Tagging;

namespace TagSelecta.Commands;

public class ReadSettings : BaseSettings { }

public class ReadCommand(IAnsiConsole console) : BaseCommand<ReadSettings>(console)
{
    protected override void Process()
    {
        var allConfirmed = false;
        for (int i = 0; i < Files.Count; i++)
        {
            var file = Files[i];
            PrintCurrentFile(file, i, Files.Count);
            TagData tagData;
            try
            {
                tagData = Tagger.ReadTags(file);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                continue;
            }
            TagDataPrinter.PrintTagData(Console, tagData);

            if (!allConfirmed && i < Files.Count - 1)
            {
                var confirmation = Console.Prompt(
                    new TextPrompt<string>("Continue? (yes, no, yes to all)")
                        .AddChoices(["y", "n", "a"])
                        .DefaultValue("y")
                );
                if (confirmation == "n")
                {
                    break;
                }
                else if (confirmation == "a")
                {
                    allConfirmed = true;
                }
            }
        }
    }
}
