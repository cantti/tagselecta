namespace TagSelecta.Cli.Commands.TagDataCommands.Common;

public class TagDataActionContext<TSettings>
{
    public required Item Current { get; set; }

    public required List<Item> Items { get; set; }

    public required TSettings Settings { get; set; }
}
