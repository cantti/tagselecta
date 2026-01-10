namespace TagSelecta.App.Tui.TuiCommands;

public record Request(string Name, Arg[] Args);

public record Arg(string Key, string Value);
