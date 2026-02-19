using Spectre.Console;
using Spectre.Console.Rendering;
using TagLib;

namespace TagSelecta.Commands.Tui.Widgets;

public class PictureWidget(TagDataActionTarget? focusedFile, int maxHeight) : Renderable
{
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var rows = new List<IRenderable>();
        rows.Add(new SectionHeaderWidget("Picture:"));

        if (focusedFile == null)
        {
            return ((IRenderable)new Rows(rows)).Render(options, maxWidth);
        }

        var picture = focusedFile
            .CurrentTagData.Picture.OrderBy(x =>
            {
                return x.Type switch
                {
                    PictureType.FrontCover => 0,
                    PictureType.BackCover => 1,
                    _ => 2,
                };
            })
            .FirstOrDefault();

        if (picture == null)
        {
            return ((IRenderable)new Rows(rows)).Render(options, maxWidth);
        }

        var image = new CanvasImage(picture.Data.ToArray());
        var width = image.Width;
        var height = image.Height;
        var scale = height > 0 ? Math.Min(1.0, (double)maxHeight / height) : 1.0;
        var newWidth = (int)Math.Floor(width * scale);

        newWidth = Math.Clamp(newWidth, 1, Math.Max(1, maxWidth));

        image.MaxWidth(newWidth);
        rows.Add(image);

        return ((IRenderable)new Rows(rows)).Render(options, maxWidth);
    }
}
