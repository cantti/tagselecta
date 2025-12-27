namespace TagSelecta.Cli.Commands.Common;

public class TagDataActionContext<TSettings>
{
    public required TagDataOperation Current { get; set; }

    public required List<TagDataOperation> Items { get; set; }

    public required TSettings Settings { get; set; }
}
