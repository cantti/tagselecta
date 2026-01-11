using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.App.IO;

namespace TagSelecta.App.TagDataActions.FileCommands;

public class FileCommand<TSettings>(
    FileAction<TSettings> action,
    IAnsiConsole console,
    IAudioFileScanner audioFileScanner
) : AsyncCommand<TSettings>
    where TSettings : BaseSettings
{
    protected override async Task<int> ExecuteAsync(
        CommandContext context,
        TSettings settings,
        CancellationToken ct
    )
    {
        AltScreen.Enter();

        console.Cursor.Hide();

        var files = audioFileScanner.Scan(settings.Path, true);

        var actionContext = new FileActionContext<TSettings>(console)
        {
            Files = files,
            Settings = settings,
        };

        var currentFileIndex = 0;
        while (true)
        {
            console.Clear();
            currentFileIndex = ClampIndex(currentFileIndex, files.Count);
            var currentFile = files[currentFileIndex];

            CommandHelper.PrintCurrentFile(console, currentFile, currentFileIndex, files.Count);
            try
            {
                actionContext.SetCurrentFile(currentFile, currentFileIndex);
                await action.ProcessFileAsync(actionContext);
            }
            catch (Exception ex)
            {
                CommandHelper.PrintStatusError(console);
                console.MarkupLineInterpolated($"[red]{ex.Message}[/]");
                continue;
            }

            var direction = ReadNavigationKey();
            if (direction == 0)
                continue;

            currentFileIndex += direction;
        }
    }

    private static int ClampIndex(int index, int count)
    {
        if (index < 0)
            return 0;
        if (index >= count)
            return count - 1;
        return index;
    }

    private int ReadNavigationKey()
    {
        console.WriteLine("N = previous, n = next");

        while (true)
        {
            var key = console.Input.ReadKey(true)?.KeyChar;
            return key switch
            {
                'j' => 1,
                'k' => -1,
                _ => 0,
            };
        }
    }
}
