using System.Globalization;
using Spectre.Console;
using TagSelecta.BaseCommands;
using TagSelecta.Tagging;

namespace TagSelecta.Commands;

public class TitleCaseSettings : FileSettings { }

public class TitleCaseCommand(IAnsiConsole console) : FileCommand<TitleCaseSettings>(console)
{
    protected override Task Execute(string file, int index)
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

        if (!TagDataChanged(originalTags, tags))
        {
            Skip();
            return Task.CompletedTask;
        }

        if (ConfirmPrompt())
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
