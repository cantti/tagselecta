namespace TagSelecta.TagDataActions.Abstractions;

public interface ISettingsWithKey
{
    public IEnumerable<string> Key { get; set; }
}

public interface ISettingsWithKeyValue : ISettingsWithKey
{
    public IEnumerable<string> Value { get; set; }
}
