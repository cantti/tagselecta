using Spectre.Console;
using Spectre.Console.Advanced;

namespace TagSelecta.Commands.Tui;

public static class AltScreen
{
    private static bool _active;

    public static bool IsActive => _active;

    public static void Enter()
    {
        if (_active)
            return;

        AnsiConsole.Console.WriteAnsi("\x1b[?1049h"); // enter alt screen
        AnsiConsole.Console.WriteAnsi("\x1b[H"); // cursor home
        _active = true;
    }

    public static void Exit()
    {
        if (!_active)
            return;

        AnsiConsole.Console.WriteAnsi("\x1b[?1049l"); // exit alt screen
        _active = false;
    }
}
