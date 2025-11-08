using System.Reflection;
using Spectre.Console;
using TagSelecta.Tagging;

namespace TagSelecta.BaseCommands;

public abstract class TagDataProcessingCommand<TSettings>(IAnsiConsole console)
    : FileProcessingCommand<TSettings>(console)
    where TSettings : BaseSettings
{
    private TagData? _tagData;
    protected TagData TagData =>
        _tagData ?? throw new InvalidOperationException("_tagData not set");

    private TagData? _originalTagData;
    private TagData OriginalTagData =>
        _originalTagData ?? throw new InvalidOperationException("_originalTagData not set");

    protected bool CompareBeforeWriteTagData { get; init; } = true;

    protected sealed override void ProcessFile() { }

    protected sealed override async Task ProcessFileAsync()
    {
        _tagData = Tagger.ReadTags(CurrentFile);
        _originalTagData = _tagData.Clone();
        await ProcessTagDataAsync();
        WriteTagDataWithConfirmation();
    }

    protected virtual void ProcessTagData() { }

    protected virtual Task ProcessTagDataAsync()
    {
        ProcessTagData();
        return Task.CompletedTask;
    }

    protected virtual void BeforeWriteTagData() { }

    private void WriteTagDataWithConfirmation()
    {
        if (CompareBeforeWriteTagData && TagDataComparer.TagDataEquals(OriginalTagData, TagData))
        {
            Console.MarkupLine("Nothing to change.");
            Skip();
        }
        else
        {
            TagDataPrinter.PrintComparison(Console, OriginalTagData, TagData);
            if (!ConfirmPrompt())
            {
                return;
            }
            BeforeWriteTagData();
            Tagger.WriteTags(CurrentFile, TagData);
        }
    }

    protected bool ValidateFieldNameList(IEnumerable<string> fields)
    {
        var tagDataProps = typeof(TagData)
            .GetProperties()
            .Where(x => x.GetCustomAttribute<EditableAttribute>() != null);
        foreach (var field in fields)
        {
            if (
                !tagDataProps.Any(x =>
                    x.Name.Equals(field, StringComparison.CurrentCultureIgnoreCase)
                )
            )
            {
                Console.MarkupLineInterpolated($"[red]Unknown field: {field}[/]");
                return false;
            }
        }
        return true;
    }

    protected List<string> NormalizeFieldNames(IEnumerable<string> list)
    {
        return [.. list.Select(x => x.ToLower().Trim())];
    }
}
