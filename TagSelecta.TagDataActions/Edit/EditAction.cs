using TagLib;
using TagSelecta.Shared.Exceptions;
using TagSelecta.Shared.Http;
using TagSelecta.Shared.IO;
using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.TagDataActions.Edit;

[TagDataActionName("edit", "e")]
public class EditAction(IDownloader downloader) : TagDataAction<EditSettings>
{
    public override async Task ExecuteAsync(
        TagDataActionExecuteContext<EditSettings> context,
        CancellationToken token
    )
    {
        var tagData = context.Target.CurrentTagData;
        var formatter = new TagDataFormatter(
            context.Target.BackupTagData,
            context.Target.BackupPath
        );

        SetStandardFields(context, tagData, formatter);

        if (context.Settings.ClearExtra)
        {
            tagData.ClearExtraFields();
        }

        SetKeyValueFields(context, formatter, tagData);

        await SetPicture(context, tagData, token);

        context.Target.UpdateTagData(tagData);
    }

    private static void SetStandardFields(
        TagDataActionExecuteContext<EditSettings> context,
        TagData tagData,
        TagDataFormatter formatter
    )
    {
        Set(s => s.Album, v => tagData.Album = v);
        Set(s => s.AlbumArtist, v => tagData.AlbumArtist = v.ToMulti());
        Set(s => s.Artist, v => tagData.Artist = v.ToMulti());
        Set(s => s.Bpm, v => tagData.Bpm = v);
        Set(s => s.CatalogNumber, v => tagData.CatalogNumber = v);
        Set(s => s.Comment, v => tagData.Comment = v);
        Set(s => s.Composer, v => tagData.Composer = v.ToMulti());
        Set(s => s.Conductor, v => tagData.Conductor = v);
        Set(s => s.Copyright, v => tagData.Copyright = v);
        Set(s => s.Date, v => tagData.Date = v);
        Set(s => s.Disc, v => tagData.Disc = v);
        Set(s => s.DiscTotal, v => tagData.DiscTotal = v);
        Set(s => s.Genre, v => tagData.Genre = v.ToMulti());
        Set(s => s.Isrc, v => tagData.Isrc = v);
        Set(s => s.Label, v => tagData.Label = v);
        Set(s => s.Publisher, v => tagData.Publisher = v);
        Set(s => s.Title, v => tagData.Title = v);
        Set(s => s.Track, v => tagData.Track = v);
        Set(s => s.TrackTotal, v => tagData.TrackTotal = v);

        void Set(Func<EditSettings, string?> get, Action<string> set)
        {
            var value = get(context.Settings);
            if (value is null)
            {
                return;
            }
            value = formatter.Format(value);
            set(value);
        }
    }

    private static void SetKeyValueFields(
        TagDataActionExecuteContext<EditSettings> context,
        TagDataFormatter formatter,
        TagData tagData
    )
    {
        if (context.Settings.Value.Length != context.Settings.Key.Length)
        {
            throw new TagSelectaException(
                "The number of keys does not match the number of values."
            );
        }

        for (var i = 0; i < context.Settings.Key.Length; i++)
        {
            var key = context.Settings.Key[i].NormalizeKey();
            var value = context.Settings.Value[i].Trim();
            value = formatter.Format(value);
            var prop = typeof(TagData)
                .GetProperties()
                .SingleOrDefault(x =>
                    x.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase)
                    && (x.PropertyType == typeof(string) || x.PropertyType == typeof(List<string>))
                );
            if (prop is not null)
            {
                if (prop.PropertyType == typeof(string))
                {
                    prop.SetValue(tagData, value);
                }
                else
                {
                    prop.SetValue(tagData, value.ToMulti());
                }
            }
            else
            {
                tagData.SetExtraField(key, value);
            }
        }
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
