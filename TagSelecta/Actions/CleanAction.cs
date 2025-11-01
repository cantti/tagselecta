using System.ComponentModel;
using System.Text.RegularExpressions;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Actions.Base;
using TagSelecta.BaseCommands;
using TagSelecta.Print;
using TagSelecta.Tagging;

namespace TagSelecta.Actions;

public class CleanSettings : FileSettings
{
    [CommandOption("--except|-e")]
    [Description(
        "Tag to keep (can be used multiple times).\nCan also be set globally using TAGSELECTA_CLEAN_EXCEPT variable (split by any non-word character)"
    )]
    public string[]? Except { get; set; }
}

public class CleanAction(IAnsiConsole console, Printer printer) : FileAction<CleanSettings>
{
    public override void Execute(ActionContext<CleanSettings> context)
    {
        var existingTags = Tagger.ReadTags(context.File);

        var tagsToKeepList = new List<string>();

        if (context.Settings.Except is not null)
        {
            tagsToKeepList = [.. context.Settings.Except];
        }
        else
        {
            var tagsToKeepVar = Environment.GetEnvironmentVariable("TAGSELECTA_CLEAN_EXCEPT");
            if (!string.IsNullOrEmpty(tagsToKeepVar))
            {
                tagsToKeepList = [.. Regex.Split(tagsToKeepVar, @"\W+")];
            }
        }

        if (tagsToKeepList.Count == 0)
        {
            console.MarkupLine("No tags to keep provided! It will remove all known tags");
        }

        tagsToKeepList = [.. tagsToKeepList.Select(x => x.ToLower().Trim())];

        var tagDataProps = typeof(TagData).GetProperties();

        // check list of tags to keep correctness
        foreach (var tagToKeep in tagsToKeepList)
        {
            if (
                !tagDataProps.Any(x =>
                    x.Name.Equals(tagToKeep, StringComparison.CurrentCultureIgnoreCase)
                )
            )
            {
                throw new ActionException($"Unknown tag: {tagToKeep})");
            }
        }

        var newTags = new TagData();

        foreach (var prop in tagDataProps)
        {
            if (tagsToKeepList.Contains(prop.Name.ToLower()))
            {
                prop.SetValue(newTags, prop.GetValue(existingTags));
            }
        }

        printer.PrintTagData(newTags);

        if (context.ConfirmPrompt())
        {
            Tagger.RemoveTags(context.File);
            Tagger.WriteTags(context.File, newTags);
        }
    }
}
