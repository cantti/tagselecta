using System.ComponentModel;
using System.Reflection;
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

public class CleanCommand(IAnsiConsole console, IConfig config)
    : FileCommand<CleanSettings>(console)
{
    private List<string> _fieldToKeepList = [];

    protected override void BeforeExecute()
    {
        _fieldToKeepList = Settings.Except?.ToList() ?? config.CleanExcept;
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

    protected override void Execute()
    {
        foreach (
            var prop in typeof(TagData)
                .GetProperties()
                .Where(x =>
                    x.GetCustomAttribute<EditableAttribute>() is not null
                    && !_fieldToKeepList.Contains(x.Name.ToLower())
                )
        )
        {
            if (prop.PropertyType == typeof(string))
            {
                prop.SetValue(TagData, "");
            }
            else if (prop.PropertyType == typeof(List<string>))
            {
                prop.SetValue(TagData, new List<string>());
            }
            else if (prop.PropertyType == typeof(List<TagLib.Picture>))
            {
                prop.SetValue(TagData, new List<TagLib.Picture>());
            }
            else if (prop.PropertyType == typeof(double?))
            {
                prop.SetValue(TagData, null);
            }
            else if (prop.PropertyType == typeof(uint))
            {
                prop.SetValue(TagData, (uint)0);
            }
        }
        WriteTags(true);
    }
}
