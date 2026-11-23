namespace TagSelecta.Shared.Tagging;

public class TagField
{
    public TagField(string key, IEnumerable<string> text)
    {
        Key = key;
        Text = text.ToList().AsReadOnly();
    }

    public string Key { get; }

    public IReadOnlyList<string> Text { get; }
}
