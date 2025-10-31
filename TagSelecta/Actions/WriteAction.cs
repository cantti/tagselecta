using System.Reflection;
using Spectre.Console.Cli;
using TagSelecta.Actions.Base;
using TagSelecta.BaseCommands;
using TagSelecta.Print;
using TagSelecta.Tagging;

namespace TagSelecta.Actions;

public class WriteSettings : FileSettings
{
    [CommandOption("--genre|-g")]
    public string[]? Genre { get; set; }

    [CommandOption("--artist|-a")]
    public string[]? Artist { get; set; }

    [CommandOption("--albumartist|-A")]
    public string[]? AlbumArtist { get; set; }

    [CommandOption("--title|-t")]
    public string? Title { get; set; }

    [CommandOption("--album|-l")]
    public string? Album { get; set; }

    [CommandOption("--year|-y")]
    public uint? Year { get; set; }

    [CommandOption("--track|-n")]
    public uint? Track { get; set; }

    [CommandOption("--tracktotal|-N")]
    public uint? TrackTotal { get; set; }

    [CommandOption("--comment|-c")]
    public string? Comment { get; set; }

    [CommandOption("--disc|-d")]
    public uint? Disc { get; set; }

    [CommandOption("--disctotal|-D")]
    public uint? DiscTotal { get; set; }

    [CommandOption("--label|-L")]
    public string? Label { get; set; }

    [CommandOption("--catalogno|-C")]
    public string? CatalogNumber { get; set; }

    [CommandOption("--bpm")]
    public uint? Bpm { get; set; }
}

public class WriteAction : FileAction<WriteSettings>
{
    public override void Execute(ActionContext<WriteSettings> context)
    {
        var tags = Tagger.ReadTags(context.File);

        var tagProps = tags.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var settingsProps = context
            .Settings.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var tagProp in tagProps)
        {
            var settingProp = settingsProps.FirstOrDefault(p => p.Name == tagProp.Name);

            if (settingProp == null)
                continue;

            var settingValue = settingProp.GetValue(context.Settings);

            if (settingValue == null)
                continue;

            if (settingValue is string[] array)
            {
                settingValue = array.Where(x => !string.IsNullOrEmpty(x)).ToList();
            }
            else if (settingValue is string s)
            {
                settingValue = s.Trim();
            }
            tagProp.SetValue(tags, settingValue);
        }
        context.Console.PrintTagData(tags);

        if (context.ConfirmPrompt())
        {
            Tagger.WriteTags(context.File, tags);
            return;
        }
    }
}
