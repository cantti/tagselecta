using TagSelecta.Shared.Tagging;

namespace TagSelecta.Shared.Tests;

public class TagFieldTests
{
    [Fact]
    public void Constructor_CopiesValuesAndExposesReadOnlyList()
    {
        var values = new List<string> { "original" };
        var field = new TagField("artist", values);

        values[0] = "changed";
        Assert.Equal("artist", field.Key);
        Assert.Equal(["original"], field.Text);
        Assert.False(field.Text is string[]);
        Assert.Throws<NotSupportedException>(() => ((IList<string>)field.Text)[0] = "changed");
    }

    [Fact]
    public void TagData_FieldValuesCannotBeChangedThroughGetters()
    {
        var values = new List<string> { "original" };
        var data = new TagData();
        data.SetValue("artist", values);
        var field = Assert.Single(data.Fields);
        var result = data.GetValue("artist");

        values[0] = "changed";
        Assert.Throws<NotSupportedException>(() => ((IList<string>)field.Text)[0] = "changed");
        Assert.Throws<NotSupportedException>(() => ((IList<string>)result)[0] = "changed");
        Assert.Equal(["original"], data.GetValue("artist"));

        data.SetValue("artist", "replacement");
        Assert.Equal(["original"], field.Text);
        Assert.Equal(["original"], result);
        Assert.Equal(["replacement"], data.GetValue("artist"));
        Assert.Empty(data.GetValue("missing"));
    }
}
