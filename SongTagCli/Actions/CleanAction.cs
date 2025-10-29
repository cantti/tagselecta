using SongTagCli.Actions.Base;
using SongTagCli.BaseCommands;
using SongTagCli.Tagging;

namespace SongTagCli.Actions;

public class CleanSettings : FileSettings { }

public class CleanAction : IAction<CleanSettings>
{
    public void Execute(ActionContext<CleanSettings> context)
    {
        var tags = Tagger.ReadTags(context.File);
        Tagger.RemoveTags(context.File);
        Tagger.WriteTags(context.File, tags);
    }
}
