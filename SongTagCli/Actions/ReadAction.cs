using SongTagCli.Actions.Base;
using SongTagCli.BaseCommands;
using SongTagCli.Misc;
using SongTagCli.Tagging;

namespace SongTagCli.Actions;

public class ReadSettings : FileSettings { }

public class ReadAction : IAction<ReadSettings>
{
    public void Execute(ActionContext<ReadSettings> context)
    {
        var tags = Tagger.ReadTags(context.File);
        context.Console.PrintTagData(tags);
        context.ContinuePrompt();
    }
}
