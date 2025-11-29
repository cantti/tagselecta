using System.ComponentModel;
using Spectre.Console.Cli;
using TagSelecta.Shared;

namespace TagSelecta.Cli.Commands.TagDataCommands;

public class ExtractPictureSettings : BaseSettings
{
    [CommandOption("--type|-t")]
    [Description(
        "Types of pictures to extract. Multiple entries can be provided using a ';' separator.\nCommon types: FrontCover, BackCover, Artist, Other"
    )]
    public string? Type { get; set; }

    [CommandOption("--output|-o")]
    [Description("Output file name")]
    public string? Output { get; set; }

    [CommandOption("--override")]
    [Description("Override files")]
    public bool Override { get; set; }

    [CommandOption("--limit|-l")]
    [Description("Limit number of files to be extracted")]
    public int? Limit { get; set; }
}

public class ExtractPictureAction : TagDataAction<ExtractPictureSettings>
{
    private readonly List<TagLib.PictureType> _types = [];

    protected override bool BeforeProcessTagData(
        TagDataActionContext<ExtractPictureSettings> context
    )
    {
        if (context.Settings.Type is not null)
        {
            var typesStr = context.Settings.Type.ToMulti();
            foreach (var typeStr in typesStr)
            {
                if (Enum.TryParse<TagLib.PictureType>(typeStr, out var type))
                {
                    _types.Add(type);
                }
            }
        }
        return true;
    }

    protected override void ProcessTagData(TagDataActionContext<ExtractPictureSettings> context)
    {
        var dir = Path.GetDirectoryName(context.CurrentFile)!;
        var pictures = new List<TagLib.Picture>();
        pictures = context
            .TagData.Picture.Where(x => _types.Count == 0 || _types.Contains(x.Type))
            .OrderBy(x =>
            {
                return x.Type switch
                {
                    TagLib.PictureType.FrontCover => 0,
                    TagLib.PictureType.BackCover => 1,
                    _ => 2,
                };
            })
            .ToList();
        for (int i = 0; i < pictures.Count; i++)
        {
            if (context.Settings.Limit.HasValue && i >= context.Settings.Limit.Value)
            {
                break;
            }

            TagLib.Picture? picture = pictures[i];
            var ext = TagLib.Picture.GetExtensionFromData(picture.Data);

            var output = context.Settings.Output;

            string baseName;
            string finalExt;

            if (!string.IsNullOrWhiteSpace(output))
            {
                // Detect if output file contains extension
                var userExt = Path.GetExtension(output);

                if (!string.IsNullOrEmpty(userExt))
                {
                    baseName = Path.GetFileNameWithoutExtension(output);
                    finalExt = userExt;
                }
                else
                {
                    baseName = output;
                    finalExt = ext;
                }
            }
            else
            {
                baseName = picture.Type.ToString();
                finalExt = ext;
            }

            var fileName = baseName + finalExt;
            var filePath = Path.Combine(dir, fileName);

            if (!context.Settings.Override)
            {
                int counter = 1;
                while (File.Exists(filePath))
                {
                    var numberedName = $"{baseName}({counter}){finalExt}";
                    filePath = Path.Combine(dir, numberedName);
                    counter++;
                }
            }

            File.WriteAllBytes(filePath, picture.Data.ToArray());
        }
    }
}
