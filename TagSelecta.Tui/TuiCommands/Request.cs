using TagSelecta.Shared.TagDataActions;

namespace TagSelecta.Tui.TuiCommands;

public record Request(string Name, TagDataActionArg[] Args);
