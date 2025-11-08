using System.ComponentModel;
using System.Reflection;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.BaseCommands;
using TagSelecta.Tagging;

namespace TagSelecta.Commands;

public class CleanSettings : BaseSettings
{
    [CommandOption("--except|-e")]
    [Description(
        "Tag to keep (can be used multiple times).\nCan also be set globally using TAGSELECTA_CLEAN_EXCEPT variable (split by any non-word character)"
    )]
    public string[]? Except { get; set; }
}

public class CleanCommand : TagDataCommand<CleanSettings>
{
    private List<string> _fieldToKeepList = [];
    private readonly IConfig config;

    public CleanCommand(IAnsiConsole console, IConfig config)
        : base(console)
    {
        this.config = config;
        CompareBeforeWriteTagData = false;
    }

    protected override void BeforeProcess()
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

    protected override void ProcessTagData()
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

        Console.MarkupLine(
            "The comparison does not display unsupported tags, which will also be removed!"
        );
    }

    protected override void BeforeWriteTagData()
    {
        Tagger.RemoveTags(CurrentFile);
    }
}
