namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("selectall")]
public class SelectAllCommand : ITuiCommand
{
    public Task ExecuteAsync(
        ITuiCommandContext context,
        ParsedCommand parsedCommand,
        CancellationToken token
    )
    {
        var filesToSelect = context.Files.Where(x => !x.IsSelected).ToList();
        foreach (var file in filesToSelect)
        {
            file.IsSelected = true;
        }

        return Task.CompletedTask;
    }
}
