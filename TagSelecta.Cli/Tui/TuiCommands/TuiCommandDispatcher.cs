using System.Reflection;

namespace TagSelecta.Cli.Tui.TuiCommands;

public class TuiCommandDispatcher : ITuiCommandDispatcher
{
    CancellationTokenSource? _currentCommandCts;
    Task? _currentCommandTask;

    private readonly List<(string[] Names, ITuiCommand command)> _commands = [];

    public TuiCommandDispatcher(IEnumerable<ITuiCommand> commands)
    {
        foreach (var command in commands)
        {
            var type = command.GetType();
            var attr = type.GetCustomAttribute<TuiCommandAttribute>();
            if (attr is null || attr.Names.Length == 0)
                continue;

            _commands.Add((attr.Names, command));
        }
    }

    public async Task DispatchAsync(
        ITuiCommandContext context,
        Request request,
        CancellationToken token
    )
    {
        var command = _commands.FirstOrDefault(c => c.Names.Contains(request.Name));
        if (command == default)
        {
            command = _commands.Single(c => c.command.GetType() == typeof(TagDataCommand));
        }

        if (command != default)
        {
            if (_currentCommandCts != null)
            {
                await _currentCommandCts.CancelAsync();
                try
                {
                    await _currentCommandTask!;
                }
                catch (OperationCanceledException)
                {
                    // expected
                }
                _currentCommandCts.Dispose();
            }

            _currentCommandCts = CancellationTokenSource.CreateLinkedTokenSource(token);

            _currentCommandTask = SafeExecuteAsync(
                command.command.ExecuteAsync(context, request, _currentCommandCts.Token)
            );
        }
    }

    private static async Task SafeExecuteAsync(Task task)
    {
        try
        {
            await task;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            // todo log
        }
    }
}
