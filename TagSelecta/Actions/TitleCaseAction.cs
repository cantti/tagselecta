using System.Globalization;
using Spectre.Console;
using TagSelecta.Actions.Base;
using TagSelecta.BaseCommands;
using TagSelecta.Print;
using TagSelecta.Tagging;

namespace TagSelecta.Actions;

public class TitleCaseSettings : FileSettings { }

public class TitleCaseAction(
    Printer printer,
    IAnsiConsole console,
    ActionContext<TitleCaseSettings> context
) : IFileAction<TitleCaseSettings>
{
    public Task Execute(string file, int index)
    {
        var originalTags = Tagger.ReadTags(file);

        var tags = originalTags.Clone();

        foreach (var prop in typeof(TagData).GetProperties())
        {
            if (prop.GetValue(tags) is string value)
            {
                prop.SetValue(tags, ToTitleCase(value));
            }
            if (prop.GetValue(tags) is List<string> valueList)
            {
                prop.SetValue(tags, valueList.Select(ToTitleCase).ToList());
            }
        }

        if (!ActionHelper.TagDataChanged(originalTags, tags, console))
        {
            context.Skip();
            return Task.CompletedTask;
        }

        printer.PrintTagData(tags);

        if (context.ConfirmPrompt())
        {
            Tagger.WriteTags(file, tags);
        }

        return Task.CompletedTask;
    }

    private static string ToTitleCase(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;
        return textInfo.ToTitleCase(input.ToLowerInvariant());
    }
}
