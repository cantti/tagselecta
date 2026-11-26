using TagSelecta.Commands.Tui;

namespace TagSelecta.Commands.Tests;

public class HotkeyMapTests
{
    [Theory]
    [InlineData(ConsoleKey.D, '\u0004', "ctrl+d", "metadatadown")]
    [InlineData(ConsoleKey.U, '\u0015', "ctrl+u", "metadataup")]
    public void Resolve_ControlLetter_ReturnsBoundCommand(
        ConsoleKey key,
        char keyChar,
        string binding,
        string command
    )
    {
        var hotkeys = new HotkeyMap();
        hotkeys.Bind(binding, command);
        hotkeys.Bind("u", "undo");

        var result = hotkeys.Resolve(new ConsoleKeyInfo(keyChar, key, false, false, true));

        Assert.Equal(command, result);
    }

    [Theory]
    [InlineData('j', false, "movedown")]
    [InlineData('J', true, "nextdir")]
    [InlineData('k', false, "moveup")]
    [InlineData('K', true, "prevdir")]
    [InlineData('u', false, "undo")]
    public void Resolve_PlainOrShiftedLetter_PreservesBinding(
        char keyChar,
        bool shift,
        string command
    )
    {
        var hotkeys = new HotkeyMap();
        hotkeys.Bind(keyChar.ToString(), command);
        hotkeys.Bind("ctrl+u", "metadataup");
        var key = Enum.Parse<ConsoleKey>(char.ToUpperInvariant(keyChar).ToString());

        var result = hotkeys.Resolve(new ConsoleKeyInfo(keyChar, key, shift, false, false));

        Assert.Equal(command, result);
    }

    [Theory]
    [InlineData(ConsoleKey.PageDown, HotkeyTokens.PageDown, "metadatadown")]
    [InlineData(ConsoleKey.PageUp, HotkeyTokens.PageUp, "metadataup")]
    public void Resolve_PageKey_ReturnsBoundCommand(ConsoleKey key, string binding, string command)
    {
        var hotkeys = new HotkeyMap();
        hotkeys.Bind(binding, command);

        var result = hotkeys.Resolve(new ConsoleKeyInfo('\0', key, false, false, false));

        Assert.Equal(command, result);
    }
}
