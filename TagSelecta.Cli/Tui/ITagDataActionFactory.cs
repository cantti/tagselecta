namespace TagSelecta.Cli.Tui;

public interface ITagDataActionFactory
{
    ITagDataAction Create(string name);
}