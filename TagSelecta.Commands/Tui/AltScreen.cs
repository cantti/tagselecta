using Spectre.Console;
using Spectre.Console.Advanced;

namespace TagSelecta.Commands.Tui;

public static class AltScreen
{
    public static bool IsActive { get; private set; }

    public static void Enter()
    {
        if (IsActive)
        {
            return;
        }

        AnsiConsole.Console.WriteAnsi("\x1b[?1049h"); // enter alt screen
        AnsiConsole.Console.WriteAnsi("\x1b[H"); // cursor home
        IsActive = true;
    }

    public static void Exit()
    {
        if (!IsActive)
        {
            return;
        }

        AnsiConsole.Console.WriteAnsi("\x1b[?1049l"); // exit alt screen
        IsActive = false;
    }
}
