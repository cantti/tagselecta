using TagSelecta.Shared.Tagging;

namespace TagSelecta.Shared.Tests;

public class TagDataFormatterTests
{
    [Theory]
    [InlineData("path", "music/song.flac")]
    [InlineData("filename", "song")]
    [InlineData("ext", "flac")]
    [InlineData("year", "2020")]
    public void Format_ComputedFieldsOverrideConflictingTags(string key, string expected)
    {
        var data = new TagData();
        data.SetValue(key, "conflicting metadata");
        data.SetValue(FieldName.Date, "2020-01-02");
        var formatter = new TagDataFormatter(data, "music/song.flac");

        Assert.Equal(expected, formatter.Format("{{ " + key + " }}"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid")]
    public void Format_YearIsEmptyWhenDateIsMissingOrInvalid(string date)
    {
        var data = new TagData();
        data.SetValue("year", "1999");
        data.SetValue(FieldName.Date, date);
        var formatter = new TagDataFormatter(data, "song.flac");

        Assert.Equal("", formatter.Format("{{ year }}"));
    }
}
