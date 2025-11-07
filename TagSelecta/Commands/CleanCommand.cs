using System.ComponentModel;
using System.Text.RegularExpressions;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.BaseCommands;
using TagSelecta.Tagging;

namespace TagSelecta.Commands;

public class CleanSettings : FileSettings
{
    [CommandOption("--except|-e")]
    [Description(
        "Tag to keep (can be used multiple times).\nCan also be set globally using TAGSELECTA_CLEAN_EXCEPT variable (split by any non-word character)"
    )]
    public string[]? Except { get; set; }
}

public class CleanCommand(IAnsiConsole console) : FileCommand<CleanSettings>(console)
{
    private List<string> _fieldToKeepList = [];

    protected override void BeforeExecute()
    {
        if (Settings.Except is not null)
        {
            _fieldToKeepList = [.. Settings.Except];
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
            Console.MarkupLine("No tags to keep provided! It will remove all tags");
        }
        _fieldToKeepList = NormalizeFieldNames(_fieldToKeepList);
        if (!ValidateFieldNameList(_fieldToKeepList))
        {
            Cancel();
        }
    }

    protected override void Execute(string file, int index)
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

        if (!TagDataChanged(existingTags, tagDataToKeep))
        {
            Skip();
            return;
        }

        if (ConfirmPrompt())
        {
            Tagger.RemoveTags(file);
            Tagger.WriteTags(file, tagDataToKeep);
        }
    }
}
