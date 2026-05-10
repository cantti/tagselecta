using TagLib;
using TagSelecta.Shared.Exceptions;
using TagSelecta.Shared.Http;
using TagSelecta.Shared.IO;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.TagDataActions.Edit;

[TagDataActionInfo("edit", "e", AllowRemainingArguments = true)]
public class EditAction(IDownloader downloader) : TagDataAction<EditSettings>
{
    public override async Task ExecuteAsync(
        TagDataActionExecuteContext<EditSettings> context,
        CancellationToken token
    )
    {
        if (context.Settings.Value.Length != context.Settings.Key.Length)
        {
            throw new TagSelectaException(
                "The number of keys does not match the number of values."
            );
        }

        var tagData = context.Target.CurrentTagData;
        var formatter = new TagDataFormatter(
            context.Target.BackupTagData,
            context.Target.BackupPath
        );

        if (context.Settings.Clear)
        {
            tagData.Clear();
        }

        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var settingProperties = typeof(EditSettings)
            .GetProperties()
            .Where(x => x.PropertyType == typeof(string))
            .ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);

        // add fields from properties
        foreach (var field in FieldName.All())
        {
            var value = settingProperties[field].GetValue(context.Settings);

            if (value is null)
            {
                continue;
            }

            var valueStr = (string)value;

            values[field] = valueStr;
        }

        // add values from key/value pairs
        for (var i = 0; i < context.Settings.Key.Length; i++)
        {
            var key = context.Settings.Key[i].NormalizeKey();
            var value = context.Settings.Value[i];
            values[key] = value;
        }

        // add remaining values
        foreach (var remaining in context.Settings.Remaining)
        {
            var key = remaining.Key.NormalizeKey();
            var value = remaining.Value;
            values[key] = value;
        }

        foreach (var (key, value) in values)
        {
            var formattedValue = formatter.Format(value);
            tagData.SetValue(key, formattedValue.SplitTagValuesIfNeeded(key));
        }

        await SetPicture(context, tagData, token);

        context.Target.UpdateTagData(tagData);
    }

    private async Task SetPicture(
        TagDataActionExecuteContext<EditSettings> context,
        TagData tagData,
        CancellationToken token
    )
    {
        if (context.Settings.ClearPicture)
        {
            tagData.Picture = [];
        }

        if (context.Settings.Picture is not null)
        {
            for (var i = 0; i < context.Settings.Picture.Length; i++)
            {
                var path = context.Settings.Picture[i];
                // try to find a corresponding picture type, or use first
                var typeStr =
                    context.Settings.PictureType?.ElementAtOrDefault(i)
                    ?? context.Settings.PictureType?.FirstOrDefault();

                Picture picture;

                if (path.StartsWith("http://") || path.StartsWith("https://"))
                {
                    picture = new Picture(await downloader.Download(path, token));
                }
                else
                {
                    path = PathUtils.Expand(path);
                    picture = new Picture(path);
                }

                picture.Type =
                    !string.IsNullOrEmpty(typeStr)
                    && Enum.TryParse<PictureType>(typeStr, true, out var type)
                        ? type
                        : PictureType.FrontCover;

                tagData.Picture.RemoveAll(x => x.Type == picture.Type);

                tagData.Picture.Add(picture);
            }
        }
    }
}
