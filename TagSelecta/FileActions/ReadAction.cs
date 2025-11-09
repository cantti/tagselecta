using Spectre.Console;
using TagSelecta.Tagging;

namespace TagSelecta.FileActions;

public class ReadSettings : BaseSettings { }

public class ReadAction(IAnsiConsole console) : IFileAction<ReadSettings>
{
    public Task<bool> ProcessTagData(FileActionContext<ReadSettings> context)
    {
        TagData tagData;
        try
        {
            tagData = Tagger.ReadTags(context.CurrentFile);
        }
        catch (Exception ex)
        {
            console.WriteLine(ex.Message);
            return Task.FromResult(false);
        }
        TagDataPrinter.PrintTagData(console, tagData);

        if (context.CurrentFileIndex < context.Files.Count - 1)
        {
            context.ContinuePrompt();
        }
        return Task.FromResult(true);
    }
}
