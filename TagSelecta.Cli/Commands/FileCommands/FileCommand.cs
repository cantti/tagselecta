using Spectre.Console;
using Spectre.Console.Cli;

namespace TagSelecta.Cli.Commands.FileCommands;

public class FileCommand<TSettings>(FileAction<TSettings> action, IAnsiConsole console)
    : AsyncCommand<TSettings>
    where TSettings : BaseSettings
{
    public override async Task<int> ExecuteAsync(
        CommandContext context,
        TSettings settings,
        CancellationToken ct
    )
    {
        AltScreen.Enter();

        console.Cursor.Hide();

        var files = CommandHelper.GetFiles(console, settings.Path);

        var actionContext = new FileActionContext<TSettings>(console)
        {
            Files = files,
            Settings = settings,
        };

        var currentFileIndex = 0;
        while (true)
        {
            currentFileIndex = ClampIndex(currentFileIndex, files.Count);
            console.Clear();
            var currentFile = files[currentFileIndex];

            CommandHelper.PrintCurrentFile(
                console,
                action.GetType().Name,
                currentFile,
                currentFileIndex,
                files.Count
            );
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
