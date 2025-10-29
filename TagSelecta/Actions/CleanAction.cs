using TagSelecta.Actions.Base;
using TagSelecta.BaseCommands;
using TagSelecta.Tagging;

namespace TagSelecta.Actions;

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
