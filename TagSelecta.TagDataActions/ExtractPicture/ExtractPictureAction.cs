using TagSelecta.Shared.TagDataActions;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.TagDataActions.ExtractPicture;

[TagDataActionName("extractpicture")]
public class ExtractPictureAction : TagDataAction<ExtractPictureSettings>
{
    private readonly List<TagLib.PictureType> _types = [];

    protected override bool BeforeExecute(ExtractPictureSettings settings)
    {
        if (settings.Type is not null)
        {
            var typesStr = settings.Type.ToMulti();
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

    protected override void Execute(TagDataActionExecuteContext<ExtractPictureSettings> context)
    {
        var tagData = context.Target.CurrentTagData;
        var dir = Path.GetDirectoryName(context.Target.BackupPath)!;
        var pictures = tagData
            .Picture.Where(x => _types.Count == 0 || _types.Contains(x.Type))
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

            var picture = pictures[i];
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
