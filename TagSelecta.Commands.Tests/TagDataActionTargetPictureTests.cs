using TagLib;
using TagSelecta.Shared.Tagging;

namespace TagSelecta.Commands.Tests;

public class TagDataActionTargetPictureTests
{
    [Fact]
    public void Pictures_AreSharedAcrossSnapshotsButListEditsCanBeUndone()
    {
        var picture = new TagPicture(new Picture { Data = new ByteVector(1, 2, 3) });
        var tagData = new TagData();
        tagData.AddPicture(picture);
        var target = new TagDataActionTarget("file.mp3", tagData);

        var current = target.CurrentTagData;
        var backup = target.BackupTagData;
        Assert.Same(picture, current.Picture[0]);
        Assert.Same(picture, backup.Picture[0]);

        current.ClearPicture();
        Assert.Single(target.CurrentTagData.Picture);
        target.UpdateTagData(current);
        Assert.True(target.HasChanges);
        Assert.Empty(target.CurrentTagData.Picture);
        Assert.Single(target.BackupTagData.Picture);

        target.Undo();
        Assert.False(target.HasChanges);
        Assert.Same(picture, target.CurrentTagData.Picture[0]);
    }
}
