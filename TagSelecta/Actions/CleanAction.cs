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

public class CleanAction(
    IAnsiConsole console,
    Printer printer,
    ActionContext<CleanSettings> context
) : IFileAction<CleanSettings>
{
    private List<string> _fieldToKeepList = [];

    public Task BeforeExecute()
    {
        if (context.Settings.Except is not null)
        {
            _fieldToKeepList = [.. context.Settings.Except];
        }
        else
        {
            var tagsToKeepVar = Environment.GetEnvironmentVariable("TAGSELECTA_CLEAN_EXCEPT");
            if (!string.IsNullOrEmpty(tagsToKeepVar))
            {
                _fieldToKeepList = [.. Regex.Split(tagsToKeepVar, @"\W+")];
            }
        }
        if (_fieldToKeepList.Count == 0)
        {
            console.MarkupLine("No tags to keep provided! It will remove all tags");
        }
        _fieldToKeepList = ActionHelper.NormalizeFields(_fieldToKeepList);
        if (!ActionHelper.ValidateFieldNameList(_fieldToKeepList, console))
        {
            context.Cancel();
        }
        return Task.CompletedTask;
    }

    public Task Execute(string file, int index)
    {
        var existingTags = Tagger.ReadTags(file);

        var tagDataToKeep = new TagData();

        foreach (var prop in typeof(TagData).GetProperties())
        {
            if (_fieldToKeepList.Contains(prop.Name.ToLower()))
            {
                prop.SetValue(tagDataToKeep, prop.GetValue(existingTags));
            }
        }

        printer.PrintTagData(tagDataToKeep);

        if (context.ConfirmPrompt())
        {
            Tagger.RemoveTags(file);
            Tagger.WriteTags(file, tagDataToKeep);
        }

        return Task.CompletedTask;
    }
}
