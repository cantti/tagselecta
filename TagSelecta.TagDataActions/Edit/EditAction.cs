using TagLib;
using TagSelecta.Shared.Exceptions;
using TagSelecta.Shared.Http;
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

        Write(s => s.Album, v => tagData.Album = v);
        Write(s => s.AlbumArtist, v => tagData.AlbumArtist = v.ToMulti());
        Write(s => s.Artist, v => tagData.Artist = v.ToMulti());
        Write(s => s.Bpm, v => tagData.Bpm = v);
        Write(s => s.CatalogNumber, v => tagData.CatalogNumber = v);
        Write(s => s.Comment, v => tagData.Comment = v);
        Write(s => s.Composer, v => tagData.Composer = v.ToMulti());
        Write(s => s.Conductor, v => tagData.Conductor = v);
        Write(s => s.Copyright, v => tagData.Copyright = v);
        Write(s => s.Date, v => tagData.Date = v);
        Write(s => s.Disc, v => tagData.Disc = v);
        Write(s => s.DiscTotal, v => tagData.DiscTotal = v);
        Write(s => s.Genre, v => tagData.Genre = v.ToMulti());
        Write(s => s.Isrc, v => tagData.Isrc = v);
        Write(s => s.Label, v => tagData.Label = v);
        Write(s => s.Publisher, v => tagData.Publisher = v);
        Write(s => s.Title, v => tagData.Title = v);
        Write(s => s.Track, v => tagData.Track = v);
        Write(s => s.TrackTotal, v => tagData.TrackTotal = v);

        if (context.Settings.ClearExtra)
        {
            tagData.ClearExtraFields();
        }

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
            WriteSet(tagData, key, value);
        }

        await SetPicture(context, tagData, token);

        context.Target.UpdateTagData(tagData);

        return;

        void Write(Func<EditSettings, string?> get, Action<string> set)
        {
            var value = get(context.Settings);
            if (value is null)
            {
                return;
            }

            var formatted = formatter.Format(value);
            set(formatted);
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
                    picture = new Picture(path);
                }

                picture.Type =
                    !string.IsNullOrEmpty(typeStr)
                    && Enum.TryParse<PictureType>(typeStr, true, out var type)
                        ? type
                        : PictureType.FrontCover;

                tagData.Picture.Add(picture);
            }
        }
    }

    private static void WriteSet(TagData tagData, string key, string value)
    {
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
