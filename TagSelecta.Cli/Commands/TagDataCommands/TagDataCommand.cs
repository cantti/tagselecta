using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Cli.Commands;
using TagSelecta.Commands;
using TagSelecta.Tagging;

namespace TagSelecta.Cli.Commands.TagDataCommands;

public class TagDataCommand<TSettings>(TagDataAction<TSettings> action, IAnsiConsole console)
    : AsyncCommand<TSettings>
    where TSettings : BaseSettings
{
    private bool _allConfirmed = false;

    public override async Task<int> ExecuteAsync(
        CommandContext context,
        TSettings settings,
        CancellationToken ct
    )
    {
        var files = CommandHelper.GetFiles(console, settings.Path);

        var actionContext = new TagDataActionContext<TSettings>
        {
            Files = files,
            Settings = settings,
        };

        if (!await action.BeforeProcessTagDataAsync(actionContext))
        {
            return 0;
        }

        for (var currentFileIndex = 0; currentFileIndex < files.Count; currentFileIndex++)
        {
            var currentFile = files[currentFileIndex];

            CommandHelper.PrintCurrentFile(console, currentFile, currentFileIndex, files.Count);
            try
            {
                var tagData = Tagger.ReadTags(currentFile);
                actionContext.SetCurrentFile(currentFile, currentFileIndex, tagData);
                var originalTagData = tagData.Clone();
                await action.ProcessTagDataAsync(actionContext);
                if (
                    action.CompareBeforeWriteTagData
                    && TagDataComparer.TagDataEquals(originalTagData, tagData)
                )
                {
                    console.MarkupLine("Nothing to change.");
                    continue;
                }
                else
                {
                    TagDataPrinter.PrintComparison(console, originalTagData, tagData);
                    if (ConfirmPrompt())
                    {
                        await action.BeforeWriteTagDataAsync(actionContext);
                        Tagger.WriteTags(currentFile, tagData);
                        CommandHelper.PrintStatusSuccess(console);
                    }
                    else
                    {
                        CommandHelper.PrintStatusSkipped(console);
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                CommandHelper.PrintStatusError(console);
                console.MarkupLineInterpolated($"[red]{ex.Message}[/]");
                continue;
            }
            console.WriteLine();
        }
        console.MarkupLineInterpolated($"[green]Finished![/]");

        return 0;
    }

    private bool ConfirmPrompt()
    {
        if (_allConfirmed)
            return true;

        var confirmation = console.Prompt(
            new TextPrompt<string>("Confirm? ([y]es/[n]o/[a]ll)".EscapeMarkup())
                .AddChoices(["y", "n", "a", "c"])
                .DefaultValue("y")
        );

        switch (confirmation)
        {
            case "y":
                return true;

            case "n":
                return false;

            case "a":
                _allConfirmed = true;
                return true;

            default:
                return false;
        }
    }
}
