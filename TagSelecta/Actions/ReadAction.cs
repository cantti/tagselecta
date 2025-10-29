using TagSelecta.Actions.Base;
using TagSelecta.BaseCommands;
using TagSelecta.Misc;
using TagSelecta.Tagging;

namespace TagSelecta.Actions;

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
