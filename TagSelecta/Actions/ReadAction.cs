using TagSelecta.Actions.Base;
using TagSelecta.BaseCommands;
using TagSelecta.Print;
using TagSelecta.Tagging;

namespace TagSelecta.Actions;

public class ReadSettings : FileSettings { }

public class ReadAction : FileAction<ReadSettings>
{
    public override void Configure(ActionConfig cfg)
    {
        cfg.ShowContinue = true;
    }

    public override void Execute(ActionContext<ReadSettings> context)
    {
        var tags = Tagger.ReadTags(context.File);
        context.Console.PrintTagData(tags);
    }
}
