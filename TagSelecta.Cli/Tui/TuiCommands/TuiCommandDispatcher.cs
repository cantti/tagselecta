using System.Reflection;

namespace TagSelecta.Cli.Tui.TuiCommands;

public class TuiCommandDispatcher : ITuiCommandDispatcher
{
    CancellationTokenSource? _currentCommandCts;
    Task? _currentCommandTask;

    public async Task DispatchAsync(
        ITuiCommand command,
        ITuiCommandContext context,
        Request request,
        CancellationToken token
    )
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
            command.ExecuteAsync(context, request, _currentCommandCts.Token),
            context
        );
    }

    private static async Task SafeExecuteAsync(Task task, ITuiCommandContext context)
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
        finally
        {
            context.UnblockUi();
        }
    }
}
