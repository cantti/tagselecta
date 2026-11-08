using Spectre.Console;
using Spectre.Console.Rendering;
using TagLib;

namespace TagSelecta.Commands.Tui.Widgets;

public class PictureWidget(TuiAppTheme theme, TagDataActionTarget? focusedFile, int maxHeight)
    : Renderable
{
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        if (focusedFile == null)
        {
            return SectionPanel
                .Create("Picture", Text.Empty, theme.Border)
                .ToSegments(options, maxWidth);
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
            return SectionPanel
                .Create("Picture", Text.Empty, theme.Border)
                .ToSegments(options, maxWidth);
        }

        var image = new CanvasImage(picture.Data.ToArray());
        var width = image.Width;
        var height = image.Height;
        var contentHeight = Math.Max(1, maxHeight - SectionPanel.VerticalPadding);
        var scale = height > 0 ? Math.Min(1.0, (double)contentHeight / height) : 1.0;
        var newWidth = (int)Math.Floor(width * scale);

        newWidth = Math.Clamp(newWidth, 1, Math.Max(1, maxWidth - SectionPanel.HorizontalPadding));

        image.MaxWidth(newWidth);
        return SectionPanel.Create("Picture", image, theme.Border).ToSegments(options, maxWidth);
    }
}
