using System.ComponentModel;
using System.Reflection;
using Spectre.Console;
using Spectre.Console.Cli;
using TagSelecta.Configuration;
using TagSelecta.Tagging;

namespace TagSelecta.Actions.TagDataActions;

public class CleanSettings : BaseSettings
{
    [CommandOption("--except|-e")]
    [Description(
        "Tag to keep (can be used multiple times).\nCan also be set globally using TAGSELECTA_CLEAN_EXCEPT variable (split by any non-word character)"
    )]
    public string[]? Except { get; set; }
}

public class CleanAction(IConfig config, IAnsiConsole console) : ITagDataAction<CleanSettings>
{
    private List<string> _fieldToKeepList = [];

    public bool CompareBeforeWriteTagData => false;

    public Task<bool> BeforeProcessTagData(TagDataActionContext<CleanSettings> context)
    {
        _fieldToKeepList = context.Settings.Except?.ToList() ?? config.CleanExcept;
        if (_fieldToKeepList.Count == 0)
        {
            console.MarkupLine("No tags to keep provided! It will remove all tags");
        }
        _fieldToKeepList = TagDataActionHelper.NormalizeFieldNames(_fieldToKeepList);
        if (!TagDataActionHelper.ValidateFieldNameList(console, _fieldToKeepList))
        {
            return Task.FromResult(false);
        }
        return Task.FromResult(true);
    }

    public Task BeforeWriteTagData(TagDataActionContext<CleanSettings> context)
    {
        Tagger.RemoveTags(context.CurrentFile);
        return Task.CompletedTask;
    }

    public Task ProcessTagData(TagDataActionContext<CleanSettings> context)
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
                prop.SetValue(context.TagData, "");
            }
            else if (prop.PropertyType == typeof(List<string>))
            {
                prop.SetValue(context.TagData, new List<string>());
            }
            else if (prop.PropertyType == typeof(List<TagLib.Picture>))
            {
                prop.SetValue(context.TagData, new List<TagLib.Picture>());
            }
            else if (prop.PropertyType == typeof(double?))
            {
                prop.SetValue(context.TagData, null);
            }
            else if (prop.PropertyType == typeof(uint))
            {
                prop.SetValue(context.TagData, (uint)0);
            }
        }

        console.MarkupLine(
            "The comparison does not display unsupported tags, which will also be removed!"
        );

        return Task.CompletedTask;
    }
}
