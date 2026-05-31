using System.Diagnostics;
using TagLib;
using TagSelecta.Shared.IO;
using File = System.IO.File;

namespace TagSelecta.Commands.Tui.TuiCommands;

[TuiCommand("openpicture")]
public class OpenPictureCommand : ITuiCommand
{
    public Task ExecuteAsync(
        ITuiCommandContext context,
        ParsedCommand parsedCommand,
        CancellationToken token
    )
    {
        if (context.FocusedFile is null)
        {
            return Task.CompletedTask;
        }

        var picture = context
            .FocusedFile.CurrentTagData.Picture.OrderBy(x =>
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
            return Task.CompletedTask;
        }

        OpenInDefaultViewer(picture.Data.ToArray(), Picture.GetExtensionFromData(picture.Data));

        return Task.CompletedTask;
    }

    private static string OpenInDefaultViewer(byte[] imageBytes, string extension = ".png")
    {
        if (imageBytes is null || imageBytes.Length == 0)
        {
            throw new ArgumentException("Image bytes are empty.", nameof(imageBytes));
        }

        if (!extension.StartsWith("."))
        {
            extension = "." + extension;
        }

        var filePath = PathUtils.Combine(PathUtils.GetTempPath(), $"{Guid.NewGuid():N}{extension}");
        File.WriteAllBytes(filePath, imageBytes);

        Process.Start(new ProcessStartInfo { FileName = filePath, UseShellExecute = true });

        return filePath; // return it so caller can delete later if desired
    }
}
